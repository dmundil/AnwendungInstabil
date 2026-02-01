using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace AnyTeller.Services
{
    public class ChaosControlFactory
    {
        private Random _random = new Random();

        public Control CreateInputControl(string fieldName, string currentValue, bool usePlaceholder, out Label label)
        {
            label = null;
            Control inputControl = new TextBox { Text = currentValue, Name = "txt" + fieldName.Replace(" ", ""), Width = 200 };

            if (usePlaceholder)
            {
                // Placeholder logic: Text is the field name if value is empty, or just put field name as placeholder? 
                // Spec says "Placeholder text within the TextBox". 
                // Since we are PRE-POPULATING data for the user to verify/edit, 
                // the placeholder might be less visible if text is present. 
                // HOWEVER, for "Search" or empty fields it makes sense.
                // For "Update User", the fields are filled.
                // If the bot needs to identify the field, it usually looks for a Label. 
                // If there is no label, it must rely on Placeholder.
                // But if text is already there, Placeholder is gone. 
                // TRICKY: Maybe we append the label to the text or use a "Watermark" that disappears on focus?
                // Or simply: The label is NOT there, and the bot has to infer from context or accessible name?
                // Let's implement a simple "Label" property on the control for accessibility, but VISUALLY hide the external label.
                // If text is present, the Visual Placeholder is hidden. 
                // Let's set the "PlaceholderText" property (standard in .NET Core WinForms).
                ((TextBox)inputControl).PlaceholderText = fieldName;
            }
            else
            {
                label = new Label { Text = fieldName, AutoSize = true, Name = "lbl" + fieldName.Replace(" ", "") };
            }

            return inputControl;
        }

        public Control WrapIncontainer(Control control, Label label)
        {
            // Randomly nest in Panel or UserControl or just return as is (if we want to vary depth).
            // But we need to return a single container that holds both (if label exists) or just the control.
            
            Panel container = new Panel();
            container.AutoSize = true;
            container.Name = "pnl" + control.Name;

            // Randomize positions within this mini-container? 
            // The "Spatial Randomization" applies to the specific Location within the "Active GroupBox".
            // So this wrapper is just for "Control Tree Complexity".
            
            if (label != null)
            {
                // Randomly place label above or below? 
                if (_random.Next(2) == 0) // Above
                {
                    label.Location = new Point(0, 0);
                    control.Location = new Point(0, label.Height + 5);
                }
                else // Left
                {
                     label.Location = new Point(0, 5);
                     control.Location = new Point(label.Width + 5, 0);
                }
                container.Controls.Add(label);
                container.Controls.Add(control);
            }
            else
            {
                control.Location = new Point(0, 0);
                container.Controls.Add(control);
            }

            // Nesting logic
            int depth = _random.Next(0, 3); // 0 to 2 extra layers
            Control current = container;
            for(int i=0; i<depth; i++)
            {
                Panel wrapper = new Panel();
                wrapper.AutoSize = true;
                wrapper.Controls.Add(current);
                current.Location = new Point(0, 0);
                current = wrapper;
            }

            return current;
        }
    }
}
