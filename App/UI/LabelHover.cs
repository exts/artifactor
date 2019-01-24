using System;
using System.Collections.Generic;
using Godot;

namespace Artifactor.App.UI
{
    public class LabelHover : Godot.Object
    {
        public bool IsHovering => _isHovering;
        private bool _isHovering;

        public bool ClickDisabled => _clickDisabled;
        private bool _clickDisabled;
        
        private Label _label;
        private Dictionary<string, string> _hoverColors = new Dictionary<string, string>();

        public Action MouseIn;
        public Action MouseOut;

        private bool _hoverDisabled;
        
        public LabelHover(Label label, string hoverIn = "", string hoverOut = "")
        {
            _label = label;
            _label.Connect("mouse_exited", this, nameof(MouseExit));
            _label.Connect("mouse_entered", this, nameof(MouseEntered));

            if(!hoverIn.Empty() && !hoverOut.Empty())
            {
                SetHoverColor(hoverIn, hoverOut);
            }   
        }

        public void DisableClickEvent()
        {
            _clickDisabled = true;
        }
        
        public void DisableLabelHover(string color = "")
        {
            _hoverDisabled = true;
            if(!color.Empty())
            {
                SetColor(color);
            }
        }

        public void SetHoverColor(string colorIn, string colorOut)
        {
            _hoverColors["in"] = colorIn;
            _hoverColors["out"] = colorOut;
        }

        public void MouseEntered()
        {
            _isHovering = true;
            
            if(_hoverDisabled) return;
            
            SetLabelColor("in");
        }

        public void MouseExit()
        {
            _isHovering = false;
            
            if(_hoverDisabled) return;
            
            SetLabelColor("out");
        }

        private void SetLabelColor(string idx)
        {
            if(_hoverColors.ContainsKey(idx))
            {
                SetColor(_hoverColors[idx]);
            }
        }

        private void SetColor(string color)
        {
            _label.Set("custom_colors/font_color", new Color(color));
        }
    }
}