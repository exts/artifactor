using System.Collections.Generic;
using System.Linq;
using Artifactor.App.Api;
using Artifactor.App.Api.Data;
using Godot;
using CardSet = Artifactor.App.Api.CardSet;

namespace Artifactor.App.Scripts
{
    public class SearchScene : Node2D
    {
        private CardLoader _cardLoader = new CardLoader();
        
        private List<CardInfo> _cards = new List<CardInfo>();
        private List<CardInfo> _cardsFound = new List<CardInfo>();
        
        private bool _searchDisabled;

        private Control _messageBox;
        private LineEdit _searchBox;
        private Node2D _cardContainer;
        private OptionButton _searchOptions;
        private RichTextLabel _description;
        private RichTextLabel _cardDescription;

        private Menu _menu;
        
        public override void _Ready()
        {
            _cardLoader.LoadCardList();
            _cards = _cardLoader.Cards;

            _cardContainer = GetNode<Node2D>("CardContainer");
            
            _description = GetNode<RichTextLabel>("Description");
            _cardDescription = GetNode<RichTextLabel>("CardDescription");
            
            _messageBox = GetNode<Control>("MessageBox");
            _messageBox.Connect("WindowClosed", this, nameof(MessageCallback));

            _searchBox = GetNode<LineEdit>("Search/Input");
            _searchBox.Connect("text_changed", this, nameof(SearchCards));

            _searchOptions = GetNode<OptionButton>("Search/Options");
            _searchOptions.Connect("item_selected", this, nameof(ShowCard));
            _searchOptions.AddItem("Select Card...", 0);

            _menu = GetNode<Menu>("Menu");
            _menu.HideBackButton();

            if(_cards.Count <= 0)
            {
                _searchDisabled = true;
                ShowMessage("No card data, please resync");
            }
        }
        
        private void ShowMessage(string message)
        {
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
        }

        public void SearchCards(string text)
        {
            if(_searchDisabled || _cards.Count <= 0)
            {
                ShowMessage("No card data, please resync");
                return;
            }

            _cardsFound.Clear();
            _searchOptions.Clear();
            _searchOptions.AddItem("Select Card...", 0);

            // remove all children
            DeleteCardData();
            HideDescription();

            // empty search
            if(text.Trim(' ').Empty() || text.Trim(' ').Length < 3) return;
            
            var cards = _cards.FindAll(c => c.Name.ToLower().Contains(text.Trim(' ').ToLower()));
            foreach(var card in cards)
            {
                _cardsFound.Add(card);
                _searchOptions.AddItem(card.Name, card.Id);
            }

            if(_cardsFound.Count != 1) return;
            
            ShowCard(1);
            _searchOptions.Select(1);
        }

        public void ShowCard(int id)
        {
            if(id == 0) return;
            
            HideDescription();
            
            var card = _cardsFound[id-1];
            if(card == null)
            {
                ShowMessage("Invalid Card Index");
                return;
            }
            
            var filePath = $"{CardSet.CachePath}/{card.Id}.png";
            using(var dir = new Directory())
            {
                if(!dir.FileExists(filePath))
                {
                    ShowMessage("Couldn't find card image, resync");
                    return;
                }
            }

            // remove all children
            DeleteCardData();
            
            // attempt to load image texture
            var sprite = _cardLoader.LoadCardImage(filePath);
            _cardContainer.AddChild(sprite);
            ShowDescription(card.GetDescBBCode());
        }

        private void DeleteCardData()
        {
            foreach(var children in _cardContainer.GetChildren().ToList())
            {
                if(children is Node child)
                {
                    _cardContainer.RemoveChild(child);
                }
            }
        }
        
        private void ShowDescription(string message)
        {
            if(message.Empty()) return;
            
            _cardDescription.BbcodeText = message;
            
            _description.Show();
            _cardDescription.Show();
        }

        private void HideDescription()
        {
            _description.Hide();
            _cardDescription.Hide();
        }
    }
}