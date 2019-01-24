using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Artifactor.App.Api;
using Artifactor.App.Api.Data;
using Godot;
using CardSet = Artifactor.App.Api.CardSet;

namespace Artifactor.App.Scripts
{
    public class DataSyncScene : Node2D
    {
        private Menu _menu;
        private Button _syncButton;
        private Control _messageBox;
        private CheckBox _forceSync;
        private Button _cancelButton;
        private ProgressBar _progressBar;

        public static bool ForceStop;
        
        private bool _syncRunning;

        private string _cardNameKey = "english";
        private string _cardImageKey = "default";

        private bool _resetProgressBar;
        private bool _disabledOnAlert;
        
        public override void _Ready()
        {
            _messageBox = GetNode<Control>("MessageBox");
            _messageBox.Connect("WindowClosed", this, nameof(MessageCallback));
            
            var container = GetNode<Control>("Container");
            _forceSync = container.GetNode<CheckBox>("Options/ForceSyncCheckbox");
            _syncButton = container.GetNode<Button>("SyncButton");
            _progressBar = container.GetNode<ProgressBar>("ProgressBar");
            _cancelButton = container.GetNode<Button>("CancelButton");

            // set signals
            _syncButton.Connect("pressed", this, nameof(SyncButtonCallback));
            _cancelButton.Connect("pressed", this, nameof(CancelButtonCallback));

            _menu = GetNode<Menu>("Menu");
            _menu.SyncLabelHover.DisableClickEvent();
            _menu.SyncLabelHover.DisableLabelHover("#6b6b6b");
        }

        public async void SyncButtonCallback()
        {
            if(_disabledOnAlert) return;
            if(_syncRunning)
            {
                ShowMessage("Sync running please wait");
                _resetProgressBar = false;
                return;
            }

            _menu.HideBackButton();
            _syncButton.Disabled = true;

            // prevent multiple clicks before operation finishes
            _syncRunning = true;
            _cancelButton.Disabled = false;
            
            _progressBar.GetNode<Node2D>("Spinner").Show();

            var expiration = new Expiration();
            if(expiration.NeedsToBeUpdated() || _forceSync.Pressed)
            {
                var cardSet = new CardSet();
                var setResp = await cardSet.FetchSetApiUrl();

                if(ForceStop)
                {
                    ShowMessage("Sync was force stopped");
                    _syncRunning = false;
                    return;
                }

                // alert user something is wrong w/ api data
                if(setResp.Count <= 0)
                {
                    ShowMessage("Something wrong w/ api data");
                    _syncRunning = false;
                    return;
                }
                
                // update expiration time
                expiration.Update(setResp.First().ExpireTime);
                
                // now update card data
                await UpdateCardDataCache(setResp);
            }
            else
            {
                ShowMessage("Data doesn't need to be resynced, check force resync to force it");
            }

            _syncRunning = false;
        }

        private async Task UpdateCardDataCache(List<SetResp> setResps)
        {   
            var cards = new List<CardInfo>();
            foreach(var setResp in setResps)
            {
                if(ForceStop) break;
                
                var setData = await CardSet.FetchCardSetData(CardSet.FormatApiUrl(setResp));
                if(setData == null) continue;
                
                foreach(var card in setData.Data.CardList)
                {
                    if(ForceStop) break;
                    var cardName = card.CardName.ContainsKey(_cardNameKey)
                        ? card.CardName[_cardNameKey]
                        : string.Empty;

                    var cardImage = card.Images.ContainsKey(_cardImageKey)
                        ? card.Images[_cardImageKey]
                        : string.Empty;
                    
                    var cardText = card.CardText.ContainsKey(_cardNameKey)
                        ? card.CardText[_cardNameKey]
                        : string.Empty;

                    if(cardName.Empty() || cardImage.Empty()) continue;
                    
                    var cardInfo = new CardInfo
                    {
                        Id = card.CardId,
                        Name = cardName,
                        Image = cardImage,
                        Description = cardText
                    };
                            
                    cards.Add(cardInfo);
                }
            }

            if(ForceStop)
            {
                ShowMessage("Sync was force stopped");
                return;
            }

            if(cards.Count <= 0)
            {
                ShowMessage("No Cards to fetch");
                _syncRunning = false;
                return;
            }

            _progressBar.MaxValue = cards.Count;
            
            // store card data in json file
            CardSet.StoreCardData(cards);
                
            // async download images
            await CardSet.DownloadCardImages(cards, _progressBar);
            
            if(ForceStop)
            {
                ShowMessage("Sync was force stopped");
                return;
            }

            // show message on completion
            ShowMessage("Download successful!");
        }

        private void ShowMessage(string message)
        {
            _disabledOnAlert = true;
            _resetProgressBar = true;
            _messageBox.GetNode<Label>("Panel/Label").Text = message;
            _messageBox.Show();
        }

        private void HideMessage()
        {
            _messageBox.Hide();
            _messageBox.GetNode<Label>("Panel/Label").Text = "";
        }
        
        public void MessageCallback()
        {
            HideMessage();
            HandleProgressReset();

            if(_syncRunning && !ForceStop) return;
            
            _menu.ShowBackButton();
            _disabledOnAlert = false;
            ForceStop = false;
            _cancelButton.Disabled = true;
            _forceSync.SetPressed(false);
            _syncButton.Disabled = false;
        }

        public void CancelButtonCallback()
        {
            // hide progress bar if it's open after an alert
            HandleProgressReset();

            ForceStop = true;
            _cancelButton.Disabled = true;
        }

        private void HandleProgressReset()
        {
            if(!_resetProgressBar) return;
            
            _progressBar.Value = 0;
            _progressBar.MaxValue = 100;
            _progressBar.GetNode<Node2D>("Spinner").Hide();
            _resetProgressBar = false;
        }
    }
}