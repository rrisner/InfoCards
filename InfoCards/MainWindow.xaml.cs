using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InfoCards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CardCollection collection;
        InfoCard activeCard;
        private List<Button> buttons;

        public MainWindow()
        {
            InitializeComponent();
            setActiveCollection(new CardCollection());
            getActiveCollection().add(new InfoCard());
            setActiveCard(getActiveCollection().cardAt(0));
            getActiveCard().updateTitle("This is my cool title!");
            getActiveCard().updateText("This is my new body text!");
            titleText.AcceptsReturn = false;
            this.Title = "InfoCard Viewer";
            loadStatusText.Text = "";
            initializeButtons();

            updateWindow();
        }
        private void setActiveCollection(CardCollection toSet)
        {
            collection = toSet;
        }
        private CardCollection getActiveCollection()
        {
            return collection;
        }
        private void setActiveCard(InfoCard newCard)
        {
            activeCard = newCard;
        }
        private InfoCard getActiveCard()
        {
            return activeCard;
        }
        private void updateWindow()
        {
            bodyText.Text = getActiveCard().getText();
            titleText.Text = getActiveCard().getTitle();
            updateActiveButtons();
            fileNameText.Text = getActiveCollection().getFileName();

            updateDescriptionTextbox();
            
        }
        private void updateDescriptionTextbox()
        {
            if (getActiveCard() != null)
            {
                descriptionText.Text = "Level " + getActiveCard().getLevel().ToString()
                + "  |  Card created: " + getActiveCard().getTimeCreated().ToString()
                + "  |  Last edited: " + getActiveCard().getTimeEdited().ToString();
            }
            else if (descriptionText != null)
            {
                descriptionText.Text = "No active card is set";
            }
            else
            {
            }
        }
        private void initializeButtons()
        {
            buttons = new List<Button>();
            buttons.Add(buttonNext);
            buttons.Add(buttonPrevious);
            buttons.Add(buttonWhy);
            buttons.Add(buttonHow);
            buttons.Add(buttonReturn);
            buttons.Add(buttonNewNext);
            buttons.Add(buttonNewPrevious);
            buttons.Add(buttonNewWhy);
            buttons.Add(buttonNewHow);
            buttons.Add(buttonDeleteCard);
            buttons.Add(buttonSave);
            buttons.Add(buttonLoad);
        }
        private void updateActiveButtons()
        {
            //Associate buttons and linked cards
            for (int i = 0; i < Enum.GetValues(typeof(InfoCard.CARD_INTERFACE)).Length; i++)
            {
                buttons[i].IsEnabled = !getActiveCard().getLinkedCardAbsID(i).Equals(InfoCard.NULL_CARD_STR);
            }
            //May not delete last remaining card
            buttonDeleteCard.IsEnabled = !getActiveCard().getAbsoluteID().Equals(getActiveCollection().cardAt(0).getAbsoluteID());

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Typing in body text box
            if (getActiveCard() != null && bodyText.IsKeyboardFocused)
            {
                getActiveCard().updateText(bodyText.Text);
                updateDescriptionTextbox();
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //save
            try
            {
                getActiveCollection().save();
                loadStatusText.Text = "Saved file successfully.";
            }
            catch (Exception)
            {
                loadStatusText.Text = "Failed to save file!";
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //load
            try
            {
                getActiveCollection().load();
                setActiveCard(getActiveCollection().cardAt(0));
                loadStatusText.Text = "Cards loaded successfully.";
            }
            catch (Exception)
            {
                loadStatusText.Text = "Load file failed - Please check file name.";
            }
            
            updateWindow();
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            //Title text box
            if (getActiveCard() != null && titleText.IsKeyboardFocused)
            {
                getActiveCard().updateTitle(titleText.Text);
                updateDescriptionTextbox();
            }
        }

        private void buttonNewPrevious_Click(object sender, RoutedEventArgs e)
        {
            //new previous
            InfoCard toAdd = new InfoCard();
            getActiveCollection().add(toAdd);

            //The null card will be protected because it is immutable
            getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.PREVIOUS)).
                                setRelation(InfoCard.CARD_INTERFACE.NEXT, toAdd);
            getActiveCard().setRelation(InfoCard.CARD_INTERFACE.PREVIOUS, toAdd);
            setActiveCard(toAdd);
            updateWindow();
            return;

            /*
            if (getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.PREVIOUS).Equals(InfoCard.NULL_CARD_STR))
            {
                //No other card exists for that relation yet
                getActiveCard().setRelation(InfoCard.CARD_INTERFACE.PREVIOUS, toAdd);
                setActiveCard(toAdd);
            }
            else
            {
                getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.PREVIOUS)).
                    setRelation(InfoCard.CARD_INTERFACE.NEXT, toAdd);
                getActiveCard().setRelation(InfoCard.CARD_INTERFACE.PREVIOUS, toAdd);
                setActiveCard(toAdd);
            }
            */
        }

        private void buttonNewNext_Click(object sender, RoutedEventArgs e)
        {
            //new next
            InfoCard toAdd = new InfoCard();
            getActiveCollection().add(toAdd);

            //The null card will be protected because it is immutable
            getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.NEXT)).
                                setRelation(InfoCard.CARD_INTERFACE.PREVIOUS, toAdd);
            getActiveCard().setRelation(InfoCard.CARD_INTERFACE.NEXT, toAdd);
            setActiveCard(toAdd);
            updateWindow();
            return;
        }

        private void buttonNewWhy_Click(object sender, RoutedEventArgs e)
        {
            //new why
            InfoCard toAdd = new InfoCard();
            getActiveCollection().add(toAdd);

            //The null card will be protected because it is immutable
            getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.WHY)).
                                updateLinkedCard(InfoCard.CARD_INTERFACE.RETURN, toAdd.getAbsoluteID());
            getActiveCard().setRelation(InfoCard.CARD_INTERFACE.WHY, toAdd);
            setActiveCard(toAdd);
            updateWindow();
            return;
        }

        private void buttonNewHow_Click(object sender, RoutedEventArgs e)
        {
            //new how
            InfoCard toAdd = new InfoCard();
            getActiveCollection().add(toAdd);

            //The null card will be protected because it is immutable
            getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.HOW)).
                                updateLinkedCard(InfoCard.CARD_INTERFACE.RETURN, toAdd.getAbsoluteID());
            getActiveCard().setRelation(InfoCard.CARD_INTERFACE.HOW, toAdd);
            setActiveCard(toAdd);
            updateWindow();
            return;
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            setActiveCard(getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.NEXT)));
            updateWindow();
        }

        private void buttonReturn_Click(object sender, RoutedEventArgs e)
        {
            setActiveCard(getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.RETURN)));
            updateWindow();
        }

        private void buttonHow_Click(object sender, RoutedEventArgs e)
        {
            setActiveCard(getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.HOW)));
            updateWindow();
        }

        private void buttonWhy_Click(object sender, RoutedEventArgs e)
        {
            setActiveCard(getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.WHY)));
            updateWindow();
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            setActiveCard(getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.PREVIOUS)));
            updateWindow();
        }

        private void buttonDeleteCard_Click(object sender, RoutedEventArgs e)
        {
            collection.removeByAbsID(getActiveCard());

            switch (getActiveCard().getDirectionToOrigin())
            {
                case InfoCard.DIRECTION_TO_ORIGIN.UNKNOWN:
                    Console.WriteLine("Warning: Dangling ID references to removed card may exist!");
                    setActiveCard(getActiveCollection().cardAt(0));
                    break;
                case InfoCard.DIRECTION_TO_ORIGIN.NEXT:
                    getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.NEXT)).setRelation(InfoCard.CARD_INTERFACE.PREVIOUS,
                        getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.PREVIOUS)));
                    setActiveCard(getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.NEXT)));
                    break;
                case InfoCard.DIRECTION_TO_ORIGIN.PREVIOUS:
                    getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.PREVIOUS)).setRelation(InfoCard.CARD_INTERFACE.NEXT,
                        getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.NEXT)));
                    setActiveCard(getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.PREVIOUS)));
                    break;
                case InfoCard.DIRECTION_TO_ORIGIN.RETURN:

                    /*if (!getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.HOW).Equals(InfoCard.NULL_CARD_STR))
                    {
                        getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.HOW)).setRelation(InfoCard.CARD_INTERFACE.RETURN,
                            getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.RETURN)));
                    }
                    else if (!getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.WHY).Equals(InfoCard.NULL_CARD_STR))
                    {
                        getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.WHY)).updateLinkedCard(InfoCard.CARD_INTERFACE.RETURN,
                            getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.RETURN)).getAbsoluteID());

                        getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.WHY)).updateLinkedCard(InfoCard.CARD_INTERFACE.RETURN,
                            getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.RETURN)).getAbsoluteID());
                    }*/

                    //else 
                    if (getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.RETURN)).getLinkedCardAbsID(InfoCard.CARD_INTERFACE.HOW).Equals(getActiveCard().getAbsoluteID()))
                    {
                        getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.RETURN)).setRelation(InfoCard.CARD_INTERFACE.HOW,
                            CardCollection.NULL_CARD);
                            //InfoCard.NULL_CARD_STR);
                    }
                    else if (getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.RETURN)).getLinkedCardAbsID(InfoCard.CARD_INTERFACE.WHY).Equals(getActiveCard().getAbsoluteID()))
                    {
                        getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.RETURN)).setRelation(InfoCard.CARD_INTERFACE.WHY,
                            CardCollection.NULL_CARD);
                            //InfoCard.NULL_CARD_STR);
                    }
                    else
                    {
                        throw new Exception("Unable to determine card nearer to origin from Return button!");
                    }

                    setActiveCard(getActiveCollection().getCardByAbsID(getActiveCard().getLinkedCardAbsID(InfoCard.CARD_INTERFACE.RETURN)));
                    break;
                default:
                    setActiveCard(getActiveCollection().cardAt(0));
                    break;
            }
            updateWindow();
        }

        private void fileNameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (getActiveCollection() != null && fileNameText.IsKeyboardFocused)
            {
                getActiveCollection().updateFileName(fileNameText.Text);
            }
        }
    }
}
