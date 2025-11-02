using System;
using System.Windows.Forms;
using System.Drawing; // Added for Point
using System.Linq; // Added for font checking

namespace Eng2Myan
{
    public partial class Eng2Myan : Form
    {
        // 1. Create an instance of the IME class.
        private readonly BurglishIME ime = new BurglishIME();
        private readonly ZawgyiConverter converter = new ZawgyiConverter(); 
        // 2. Declare your ListBox
        private ListBox suggestionDropdown;

        public Eng2Myan()
        {
            InitializeComponent();

            // --- FONT INITIALIZATION ---
            // The transliteration logic is hard-coded for Zawgyi encoding.
            // We MUST use a Zawgyi font to display the characters correctly.
            Font myanmarFont;
            string fontName = "Zawgyi-One";

            try
            {
                // Check if Zawgyi-One is installed
                bool isFontInstalled = new System.Drawing.Text.InstalledFontCollection().Families.Any(f => f.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));

                if (isFontInstalled)
                {
                    myanmarFont = new Font(fontName, 12f);
                }
                else
                {
                    // Fall back to a default font
                    myanmarFont = new Font("Arial", 12f);
                    // Show a warning to the user
                    MessageBox.Show(
                        $"The font '{fontName}' is not installed.\n\nThis application requires '{fontName}' to display Burmese characters correctly. Please install the font and restart the application.",
                        "Font Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                // General fallback
                myanmarFont = new Font("Arial", 12f);
                MessageBox.Show($"An error occurred while loading the font: {ex.Message}", "Font Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Apply the font to all relevant controls
            this.usrInput.Font = myanmarFont;
            this.transliterateOutput.Font = myanmarFont;
            // --- END FONT INITIALIZATION ---


            // 3. Initialize and configure the ListBox
            suggestionDropdown = new ListBox();
            suggestionDropdown.Font = myanmarFont; // SET FONT HERE
            suggestionDropdown.Visible = false; // Start hidden
            suggestionDropdown.Width = 150; // Set a default width
            suggestionDropdown.Height = 100; // Set a default height

            // Position it below the usrInput. You may need to adjust this.
            suggestionDropdown.Location = new Point(usrInput.Left, usrInput.Bottom + 2);

            // 4. Add the event handler for when a user CLICKS an item
            suggestionDropdown.MouseClick += new MouseEventHandler(suggestionDropdown_MouseClick);

            // 5. Add the ListBox to the form's controls
            this.Controls.Add(suggestionDropdown);
            suggestionDropdown.BringToFront(); // Make sure it's on top

            // 6. Add the KeyDown event handler for the input box
            // This is the key to intercepting 'Enter', 'Up', 'Down', etc.
            this.usrInput.KeyDown += new KeyEventHandler(this.usrInput_KeyDown);
        }

        private void usrInput_TextChanged(object sender, EventArgs e)
        {
            string currentText = usrInput.Text;

            if (string.IsNullOrWhiteSpace(currentText))
            {
                // Clear the output if the input is empty
                // transliterateOutput.Text = ""; // We don't clear output anymore
                // Clear and hide the dropdown
                suggestionDropdown.Items.Clear();
                suggestionDropdown.Visible = false;
            }
            else
            {
                // --- Dropdown List Implementation ---

                // 1. Get all suggestions
                var suggestions = ime.GetRawSuggestions(currentText);

                // 2. Clear the dropdown
                suggestionDropdown.Items.Clear();

                // 3. Add new suggestions
                foreach (string suggestion in suggestions)
                {
                    suggestionDropdown.Items.Add(suggestion);
                }

                // 4. Show the dropdown if it has items
                if (suggestionDropdown.Items.Count > 0)
                {
                    suggestionDropdown.SelectedIndex = 0; // Highlight the first one
                    suggestionDropdown.Visible = true;
                }
                else
                {
                    suggestionDropdown.Visible = false;
                }
            }
        }

        // 7. This event handles key presses in the input box
        private void usrInput_KeyDown(object sender, KeyEventArgs e)
        {
            // Only run this logic if the dropdown is visible
            if (suggestionDropdown.Visible)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        // 'Enter' key
                        e.Handled = true; // Mark event as handled
                        e.SuppressKeyPress = true; // Stop the "ding" sound and newline
                        AcceptSuggestion(addSpace: true); // Changed from addNewLine: true
                        break;

                    case Keys.Space: // ADDED THIS CASE
                        // 'Space' key
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        AcceptSuggestion(addSpace: true);
                        break;

                    case Keys.Down:
                        // 'Down' arrow
                        e.Handled = true;
                        if (suggestionDropdown.SelectedIndex < suggestionDropdown.Items.Count - 1)
                        {
                            suggestionDropdown.SelectedIndex++;
                        }
                        else
                        {
                            suggestionDropdown.SelectedIndex = 0; // Wrap to top
                        }
                        break;

                    case Keys.Up:
                        // 'Up' arrow
                        e.Handled = true;
                        if (suggestionDropdown.SelectedIndex > 0)
                        {
                            suggestionDropdown.SelectedIndex--;
                        }
                        else
                        {
                            suggestionDropdown.SelectedIndex = suggestionDropdown.Items.Count - 1; // Wrap to bottom
                        }
                        break;

                    case Keys.Escape:
                        // 'Escape' key
                        e.Handled = true;
                        suggestionDropdown.Visible = false;
                        suggestionDropdown.Items.Clear();
                        break;
                }
            }
            else if (e.KeyCode == Keys.Space) // Handle space even if dropdown is hidden
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                // Just add a space to the output and clear the input
                transliterateOutput.Text += " ";
                usrInput.Clear();
            }
            else if (e.KeyCode == Keys.Enter) // Handle Enter even if dropdown is hidden
            {
                // Just add a space to the output and clear the input
                e.Handled = true;
                e.SuppressKeyPress = true;
                transliterateOutput.Text += " ";
                usrInput.Clear();
            }
        }

        // 8. This event is called when the user CLICKS an item
        private void suggestionDropdown_MouseClick(object sender, MouseEventArgs e)
        {
            // Accept the suggestion and add a space, as if they hit 'Space'
            AcceptSuggestion(addSpace: true);
        }

        // 9. A helper method to accept the selected suggestion
        private void AcceptSuggestion(bool addSpace = false) // Removed addNewLine parameter
        {
            if (suggestionDropdown.Visible && suggestionDropdown.SelectedItem != null)
            {
                // When the user selects a word, APPEND it to the output box
                transliterateOutput.Text += suggestionDropdown.SelectedItem.ToString();
            }
            else
            {
                // If no suggestion is selected, just use the raw input text
                transliterateOutput.Text += usrInput.Text;
            }

            // Add a space if requested
            if (addSpace)
            {
                transliterateOutput.Text += "";
                unicodeOut.Text = converter.ToUnicode(transliterateOutput.Text);
            }

            // Removed the addNewLine block

            // Hide dropdown and CLEAR the input box for the next word
            suggestionDropdown.Visible = false;
            usrInput.Clear(); // This will fire TextChanged, which will hide the list
        }
    }
}

