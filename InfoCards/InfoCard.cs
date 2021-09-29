using System;
using System.Collections.Generic;
using System.Text;

namespace InfoCards
{
    class InfoCard
    {
        private static int count = 0;
        public static string NULL_CARD_STR = "NULL CARD";
        private static string DEFAULT_TITLE = "Untitled Card";
        private static string DEFAULT_BODY_TEXT = "Replace with your text!";
        private static string INFOCARD_HEADER_TEXT = "INFOCARD::";
        public enum CARD_INTERFACE
        {
            NEXT = 0,
            PREVIOUS = 1,
            WHY = 2,
            HOW = 3,
            RETURN = 4
        }
        public enum CARD_TYPE
        {
            DEFAULT,
            BULLETIN,
            DESCRIPTION,
            NULL_CARD
        }
        public enum DIRECTION_TO_ORIGIN
        {
            IS_ORIGIN,
            UNKNOWN,
            NEXT,
            PREVIOUS,
            RETURN
        }

        private string absoluteID;
        private int ID; //Relative for this session
        private string title;
        private string text;
        private int lines;
        private int level;
        private DIRECTION_TO_ORIGIN directionToOrigin;
        private DateTime timeCreated;
        private DateTime timeEdited;
        private List<string> linkedCards;
        private CARD_TYPE cardType;

        public InfoCard()
        {
            setTimeCreated(DateTime.Now);
            setID(nextID());
            generateAbsoluteID();
            initializeLinkedCards();
            setCardType(CARD_TYPE.DEFAULT);
            setTitle(DEFAULT_TITLE);
            setText(DEFAULT_BODY_TEXT);
            setLevel(0);
            setDirectionToOrigin(DIRECTION_TO_ORIGIN.UNKNOWN);
        }

        public InfoCard(System.IO.StreamReader streamReader)
        {
            setID(nextID());
            FromFile(streamReader);
        }
        public InfoCard(string specialID)
        {
            setTimeCreated(DateTime.Now);
            setID(nextID());
            setAbsoluteID(specialID);
            initializeLinkedCards();

            if (specialID.Equals(NULL_CARD_STR))
            {
                setCardType(CARD_TYPE.NULL_CARD);
                setTitle(NULL_CARD_STR);
                setText(NULL_CARD_STR);
            }
            else
            {
                setCardType(CARD_TYPE.DEFAULT);
                setTitle(DEFAULT_TITLE);
                setText(DEFAULT_BODY_TEXT);
            }
            
            
        }

        private void setTimeCreated(DateTime toSet)
        {
            timeCreated = toSet;
            setTimeEdited(toSet);
        }
        public DateTime getTimeCreated()
        {
            return timeCreated;
        }
        private void setTimeEdited(DateTime toSet)
        {
            timeEdited = toSet;
        }
        private void updateEditTime()
        {
            setTimeEdited(DateTime.Now);
        }
        public DateTime getTimeEdited()
        {
            return timeEdited;
        }
        private void generateAbsoluteID()
        {
            absoluteID = DateTime.Now.ToString() + getID().ToString();
            Console.WriteLine(DateTime.Now.ToString() + getID().ToString());
        }
        public string getAbsoluteID()
        {
            return absoluteID;
        }
        private void setAbsoluteID(string ID)
        {
            absoluteID = ID;
        }
        private void setID(int toSet)
        {
            ID = toSet;
        }
        private int getID()
        {
            return ID;
        }
        private static int nextID()
        {
            return ++count;
        }
        private static int cardsConstructed()
        {
            return count;
        }
        private void setLevel(int newLevel)
        {
            level = newLevel;
        }
        public int getLevel()
        {
            return level;
        }
        private void setText(string toSet)
        {
            int lines = 1;
            text = toSet;
            foreach (char every in text.ToCharArray())
            {
                if (every.Equals('\n'))
                {
                    lines++;
                }
            }
            setLines(lines);
        }
        public void updateText(string toSet)
        {
            setText(toSet);
            updateEditTime();
        }
        public string getText()
        {
            return text;
        }
        private void setTitle(string toSet)
        {
            title = toSet;
        }
        public void updateTitle(string toSet)
        {
            setTitle(toSet);
            updateEditTime();
        }
        public string getTitle()
        {
            return title;
        }
        public void setDirectionToOrigin(DIRECTION_TO_ORIGIN toSet)
        {
            directionToOrigin = toSet;
        }
        public DIRECTION_TO_ORIGIN getDirectionToOrigin()
        {
            return directionToOrigin;
        }
        private void initializeLinkedCards()
        {
            linkedCards = new List<string>();
            foreach (CARD_INTERFACE x in Enum.GetValues(typeof(CARD_INTERFACE)))
            {
                linkedCards.Add(NULL_CARD_STR);
            }
        }
        private bool setLinkedCard(int index, string absID)
        {
            if (index >= 0 && index < Enum.GetValues(typeof(CARD_INTERFACE)).Length)
            {
                linkedCards.RemoveAt(index);
                linkedCards.Insert(index, absID);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool updateLinkedCard(int index, string absID)
        {
            if (setLinkedCard(index, absID))
            {
                updateEditTime();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool updateLinkedCard(CARD_INTERFACE relation, string absID)
        {
            return updateLinkedCard((int)relation, absID);
        }
        public string getLinkedCardAbsID(int index)
        {
            if (index >= 0 && index < linkedCards.Count)
            {
                return linkedCards[index];
            }
            else
            {
                throw new Exception("Linked card does not exist for index " + index.ToString());
                //return NULL_CARD_STR;
            }

        }
        public string getLinkedCardAbsID(CARD_INTERFACE relation)
        {
            return getLinkedCardAbsID((int)relation);
        }
        public void setRelation(CARD_INTERFACE relation, InfoCard other)
        {
            if (this.getAbsoluteID().Equals(NULL_CARD_STR))
            {
                //The null card is immutable
                return;
            }

            switch (relation)
            {
                case CARD_INTERFACE.NEXT:
                    this.updateLinkedCard(CARD_INTERFACE.NEXT, other.getAbsoluteID());
                    other.updateLinkedCard(CARD_INTERFACE.PREVIOUS, this.getAbsoluteID());
                    if (getDirectionToOrigin().Equals(DIRECTION_TO_ORIGIN.NEXT)
                        || getDirectionToOrigin().Equals(DIRECTION_TO_ORIGIN.PREVIOUS))
                    {
                        other.setDirectionToOrigin(getDirectionToOrigin());
                    }
                    else if (getDirectionToOrigin().Equals(DIRECTION_TO_ORIGIN.IS_ORIGIN))
                    {
                        other.setDirectionToOrigin(DIRECTION_TO_ORIGIN.PREVIOUS);
                    }
                    break;
                case CARD_INTERFACE.PREVIOUS:
                    this.updateLinkedCard(CARD_INTERFACE.PREVIOUS, other.getAbsoluteID());
                    other.updateLinkedCard(CARD_INTERFACE.NEXT, this.getAbsoluteID());
                    if (getDirectionToOrigin().Equals(DIRECTION_TO_ORIGIN.NEXT) 
                        || getDirectionToOrigin().Equals(DIRECTION_TO_ORIGIN.PREVIOUS))
                    {
                        other.setDirectionToOrigin(getDirectionToOrigin());
                    }
                    else if (getDirectionToOrigin().Equals(DIRECTION_TO_ORIGIN.IS_ORIGIN))
                    {
                        other.setDirectionToOrigin(DIRECTION_TO_ORIGIN.NEXT);
                    }
                    break;
                case CARD_INTERFACE.WHY:
                    this.updateLinkedCard(CARD_INTERFACE.WHY, other.getAbsoluteID());
                    other.updateLinkedCard(CARD_INTERFACE.RETURN, this.getAbsoluteID());
                    other.setDirectionToOrigin(DIRECTION_TO_ORIGIN.RETURN);
                    other.setLevel(this.getLevel() + 1);
                    break;
                case CARD_INTERFACE.HOW:
                    this.updateLinkedCard(CARD_INTERFACE.HOW, other.getAbsoluteID());
                    other.updateLinkedCard(CARD_INTERFACE.RETURN, this.getAbsoluteID());
                    other.setDirectionToOrigin(DIRECTION_TO_ORIGIN.RETURN);
                    other.setLevel(this.getLevel() + 1);
                    break;

                case CARD_INTERFACE.RETURN:
                    throw new Exception("Illegal relation modifier was specified : RETURN. Unable to determine forward relation.");
                default:
                    throw new Exception("Illegal relation modifier was specified: " + relation.ToString());
            }
        }
        private void setCardType(CARD_TYPE toSet)
        {
            cardType = toSet;
        }
        private CARD_TYPE getCardType()
        {
            return cardType;
        }
        private void setLines(int toSet)
        {
            lines = toSet;
        }
        private int getLines()
        {
            return lines;
        }

        public override string ToString()
        {
            string toReturn = "";
            toReturn += INFOCARD_HEADER_TEXT + "\n";
            toReturn += getAbsoluteID() + "\n";
            toReturn += (int)getCardType() + "\n";
            toReturn += (int)getDirectionToOrigin() + "\n";
            toReturn += getLevel().ToString() + "\n";

            toReturn += getTimeCreated().ToString() + "\n";
            toReturn += getTimeEdited().ToString() + "\n";
            toReturn += getTitle() + "\n";
            
            toReturn += getLines().ToString() + "\n";
            toReturn += getText() + "\n";
            
            toReturn += linkedCards.Count.ToString() + "\n";
            foreach (string every in linkedCards)
            {
                toReturn += every + "\n";
            }

            return toReturn;
        }
        private void FromFile(System.IO.StreamReader reader)
        {
            if (reader.ReadLine().Equals(INFOCARD_HEADER_TEXT))
            {
                Console.WriteLine("Valid data");
            }
            setAbsoluteID(reader.ReadLine());
            setCardType((CARD_TYPE)int.Parse(reader.ReadLine()));
            setDirectionToOrigin((DIRECTION_TO_ORIGIN)int.Parse(reader.ReadLine()));
            setLevel(int.Parse(reader.ReadLine()));

            setTimeCreated(DateTime.Parse(reader.ReadLine()));
            setTimeEdited(DateTime.Parse(reader.ReadLine()));
            setTitle(reader.ReadLine());

            //Text body
            int linesOfText = int.Parse(reader.ReadLine());
            bool firstLine = true;
            string ofText = "";
            for (int i = 0; i < linesOfText; i++)
            {
                if (!firstLine)
                {
                    ofText += "\n";
                }
                else
                {
                    firstLine = false;
                }
                ofText += reader.ReadLine();
            }
            setText(ofText);

            //Linked cards
            initializeLinkedCards();
            linesOfText = int.Parse(reader.ReadLine());
            ofText = "";
            for (int i = 0; i < linesOfText; i++)
            {
                setLinkedCard(i, reader.ReadLine());
            }

        }

    }
}
