/**
 * Richard Risner
 * September 29, 2021
 * InfoCard Application
 * Written in C# using Visual Studio - WPF
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace InfoCards
{
    class CardCollection
    {
        private static int count = 0;
        private static string DEFAULT_TITLE = "Untitled InfoCard Collection";
        private static string COLLECTION_HEADER_TEXT = "CARDCOLLECTION::";
        private static string DEFAULT_FILE_NAME = "My InfoCard Deck.deck";
        private static string DEFAULT_SAVE_DIRECTORY = "./";

        private string absoluteID;
        private int ID; //Relative for this session
        private string title;
        private string fileName;
        private string directory;
        private DateTime timeCreated;
        private DateTime timeEdited;
        private List<InfoCard> collection;

        public CardCollection()
        {
            setTimeCreated(DateTime.Now);
            setID(nextID());
            generateAbsoluteID();
            setTitle(DEFAULT_TITLE);
            setDirectory(DEFAULT_SAVE_DIRECTORY);
            setFileName(DEFAULT_FILE_NAME);
            clear(); // new infocard collection list
        }

        private void setTimeCreated(DateTime toSet)
        {
            timeCreated = toSet;
            setTimeEdited(toSet);
        }
        private DateTime getTimeCreated()
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
        private DateTime getTimeEdited()
        {
            return timeEdited;
        }
        private void generateAbsoluteID()
        {
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
        private void setFileName(string toSet)
        {
            fileName = toSet;
        }
        public string getFileName()
        {
            return fileName;
        }
        public void updateFileName(string toSet)
        {
            updateEditTime();
            setFileName(toSet);
        }
        private void setDirectory(string toSet)
        {
            directory = toSet;
        }
        public string getDirectory()
        {
            return directory;
        }
        public void updateDirectory(string toSet)
        {
            updateEditTime();
            setDirectory(toSet);
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
        private string getTitle()
        {
            return title;
        }

        public int size()
        {
            return collection.Count;
        }
        public bool add(InfoCard toAdd)
        {
            collection.Add(toAdd);

            if (collection.Count == 1)
            {
                collection[0].setDirectionToOrigin(InfoCard.DIRECTION_TO_ORIGIN.IS_ORIGIN);
            }

            return true;
        }
        public void clear()
        {
            collection = new List<InfoCard>();
        }
        public InfoCard cardAt(int index)
        {
            if (index >= 0 && index < size())
            {
                return collection[index];
            }
            else
            {
                throw new Exception("No InfoCard exists at index " + index.ToString() + "!");
                //return NULL_CARD;
            }
        }
        public InfoCard getCardByAbsID(string absID)
        {
            if (absID.Equals(InfoCard.NULL_CARD_STR))
            {
                return NULL_CARD;
            }

            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].getAbsoluteID().Equals(absID))
                {
                    return collection[i];
                }
            }
            throw new Exception("Referenced InfoCard " + absID + " does not exist in this collection!");
            //return NULL_CARD;
        }
        public bool removeByAbsID(InfoCard toRemove)
        {
            if (collection.Contains(toRemove))
            {
                collection.Remove(toRemove);
                return true;
            }
            throw new Exception("Specified card " + toRemove.getAbsoluteID() + " does not exist in this collection!");
            //return false;
        }

        public override string ToString()
        {
            string toReturn = "";
            toReturn += COLLECTION_HEADER_TEXT + "\n";
            toReturn += getAbsoluteID() + "\n";

            toReturn += getTimeCreated().ToString() + "\n";
            toReturn += getTimeEdited().ToString() + "\n";
            toReturn += getTitle() + "\n";

            toReturn += size().ToString() + "\n";
            
            for (int i = 0; i < size(); i++)
            {
                toReturn += cardAt(i).ToString();
            }

            return toReturn;
        }
        private void FromFile(System.IO.StreamReader reader)
        {
            if (reader.ReadLine().Equals(COLLECTION_HEADER_TEXT))
            {
                Console.WriteLine("Valid data");
            }
            setAbsoluteID(reader.ReadLine());

            setTimeCreated(DateTime.Parse(reader.ReadLine()));
            setTimeEdited(DateTime.Parse(reader.ReadLine()));
            setTitle(reader.ReadLine());

            //Build Collection of card objects
            int cardCount = int.Parse(reader.ReadLine());
            for (int i = 0; i < cardCount; i++)
            {
                add(new InfoCard(reader));
            }
        }

        public bool save()
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter(getDirectory() + getFileName());
            writer.Write(ToString());
            writer.Close();
            return true;
        }

        public bool load()
        {
            System.IO.StreamReader writer = new System.IO.StreamReader(getDirectory() + getFileName());
            clear(); // load as replacement
            FromFile(writer);
            writer.Close();
            return true;
        }

        public static InfoCard NULL_CARD = new InfoCard(InfoCard.NULL_CARD_STR);

    }
}
