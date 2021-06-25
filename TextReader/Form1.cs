using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text.Json.Serialization;

namespace TextReader
{
    
    public partial class Form1 : Form
    {
        class bigData
        {
            public string fileName { get; set; }
            public int fileSize { get; set; }
            public int lettersCount { get; set; }
            public Dictionary<string, int> letters { get; set; }
            public int wordsCount { get; set; }
            public Dictionary<string, int> words { get; set; }
            public int numbersCount { get; set; }
            public Dictionary<string, int> numbers { get; set; }
            public int numeralsCount { get; set; }
            public Dictionary<string, int> numerals { get; set; }
            public int linesCount { get; set; }
            public string longestWord { get; set; }
            public int wordsWithHyphen { get; set; }
            public int punctuations { get; set; }
            public int spaceCount { get; set; }
            public int hyphenCount { get; set; }
            public void Clear()
            {
                fileName = "";
                fileSize = 0;
                lettersCount = 1;
                letters.Clear();
                wordsCount = 0;
                words.Clear();
                numbers.Clear();
                numbersCount = 0;
                numeralsCount = 0;
                numerals.Clear();
                linesCount = 0;
                longestWord = "";
                wordsWithHyphen = 0;
                punctuations = 0;
                spaceCount = 0;
                hyphenCount = 0;
            }
            public void Start()
            {
                linesCount = 1;
                letters = new Dictionary<string, int>();
                words = new Dictionary<string, int>();
                numbers = new Dictionary<string, int>();
                numerals = new Dictionary<string, int>();
            }
            public void Sort()
            {
                letters = letters.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                words = words.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                numbers = numbers.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                numerals = numerals.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            public void Sum()
            {
                lettersCount = letters.Sum(x => x.Value);
                numeralsCount = numerals.Sum(x => x.Value);
                wordsCount = words.Sum(x => x.Value);
                numbersCount = numbers.Sum(x => x.Value);
            }
        }
        bigData _Data;                  //Тут хранятся все собранные вместе и отсортированные данные
        string allText = "";
        public Form1()
        {
            InitializeComponent();
            _Data = new bigData();
            _Data.Start();
        }
        public void ClearMemory()
        {
            long totalMemory = GC.GetTotalMemory(false);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        private void button3_Click(object sender, EventArgs e) //Scan File
        {
            richTextBox2.Clear();
            _Data.Clear(); ;
            _Data.fileName = FileName(openFileDialog1.FileName);
            _Data.fileSize = allText.Length;
            string json="";
            try
            {
                textReader(richTextBox1.Text);
                 json = JsonConvert.SerializeObject(_Data, Formatting.Indented);
            }
            catch { }
            richTextBox2.Text=json;
            ClearMemory();
        }
        public string FileName(string name)
        {
            int index=0;
            string backName = "";
            for(int i=name.Length-1;i>-1; i--)
            {
                if (name[i].ToString()=="\\")
                { index = i;
                    break;
                }
            }
            for (int i = index+1; i < name.Length; i++)
                backName = backName + name[i];
            return backName;
        }//Процедура для нахождения имени файла
        private void button1_Click(object sender, EventArgs e) //Open File
        {
            openFileDialog1.Filter = "txt files (*.txt)|*.txt;";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            allText = System.IO.File.ReadAllText(filename);
            richTextBox1.Text = allText;
            ClearMemory();
        }
        private void button2_Click(object sender, EventArgs e) //ClearFile
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
            _Data.Clear();
        }
        private  void button4_Click(object sender, EventArgs e) //Safe File
        {
            saveFileDialog1.Filter = "json files (*.json)|*.json;";
            saveFileDialog1.FileName = "Result";
            saveFileDialog1.InitialDirectory = "C:\\";
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            // получаем выбранный файл
            string filename = saveFileDialog1.FileName;
            // сохраняем текст в файл
            string json = JsonConvert.SerializeObject(_Data, Formatting.Indented);
            File.WriteAllText(filename, json);
            MessageBox.Show("Файл сохранен");
        }
        public void textReader(string text)
        {
            int space1 = -1;
            int space2 = 0;
            string word="";
            bool isWord = false;
            bool isHyphen = false;
            for (int i = 0; i < text.Length; i++)
            {
                //Работа с буквами
                if (char.IsLetter(text[i]) && _Data.letters.ContainsKey(text[i].ToString()))        
                {
                    _Data.letters[text[i].ToString()] = _Data.letters[text[i].ToString()] + 1;
                }//Проверка есть ли буква в списке, если есть то +1 
                else if(char.IsLetter(text[i]) && _Data.letters.ContainsKey(text[i].ToString())==false)
                {
                    _Data.letters.Add(text[i].ToString(), 1);
                } //Проверка есть ли буква в списке, если нет - добавить в список     

                //Работа с цифрами
                if (char.IsNumber(text[i]) && _Data.numerals.ContainsKey(text[i].ToString()))
                {
                    _Data.numerals[text[i].ToString()] = _Data.numerals[text[i].ToString()] + 1;
                }//Проверка есть ли цифра в списке, если есть то +1 
                else if (char.IsNumber(text[i]) && _Data.numerals.ContainsKey(text[i].ToString())==false)
                {
                    _Data.numerals.Add(text[i].ToString(), 1);
                } //Проверка есть ли цифра в списке, если нет - добавить в список     

                //Работа с словами, словами, дефисами и числами
                if (i > space1&&char.IsLetter(text[i]) && space2 == 0
                    ||text[i].ToString()=="-"&& i > space1 && space2 == 0
                    ||char.IsPunctuation(text[i]) && i > space1 && space2 == 0)
                {
                    isWord = true;
                }//Проверка есть ли хоть одна буква, дефис или знак препинания в слове
                if ((i > space1 && text[i].ToString()==("-") && space2 == 0))
                {
                    isHyphen = true;
                }//Проверка есть ли в слове дефис
                if (space2==0 &&i>space1&& char.IsWhiteSpace(text[i])!=true&&text[i].ToString()!=("\\r"))
                {
                    word = word + text[i];
                }   //Добавление буквы в слово
                if(char.IsWhiteSpace(text[i])&&space1==i-1|| text[i].ToString() == ("\\r") && space1 == i - 1)
                {
                    space1++; 
                }   //Проверка двойных пробелов
                if (char.IsWhiteSpace(text[i]) && i > space1 || text[i].ToString() == ("\\r") && i > space1  
                    || i==text.Length-1 && i > space1)
                {
                    space2 = i;
                }//Находим второй пробел (учитывая перенос строки,конец текста и двойнные пробелы)
                if (space2>0)
                {
                    if (isWord)
                    {
                        if (_Data.words.ContainsKey(word))
                        {
                            _Data.words[word] = _Data.words[word] + 1;
                        }
                        else
                            _Data.words.Add(word, 1);
                        if (word.Length > _Data.longestWord.Length)
                            _Data.longestWord = word;                 //Проверка на самое длинное слово
                        if (isHyphen) { _Data.wordsWithHyphen++;}    //Проверка на дефис
                    }
                    if (isWord==false)
                    {
                        if (_Data.numbers.ContainsKey(word))
                        {
                            _Data.numbers[word] = _Data.numbers[word] + 1;
                        }
                        else
                            _Data.numbers.Add(word, 1);
                    }
                    space1 = space2;
                    space2 = 0;
                    isWord = false;
                    isHyphen = false;
                    word = "";
                } //записываем слово или число в массив и обнуляем второй пробел, поле isWord и isHyphen

                //Работа со знаками пунктуации
                if (char.IsPunctuation(text[i]))
                {
                    _Data.punctuations++;
                }     //Поиск количества знаков припинания
                if( text[i].ToString() == ("\n"))
                {
                    //Environment.NewLine
                    _Data.linesCount++;
                }    //Поиск количества строк
                if (text[i].ToString() == " ")
                {
                    _Data.spaceCount++;
                }       //Поиск количества пробелов
                if(text[i].ToString()=="-")
                {
                    _Data.hyphenCount++;
                }          //Проверка количества дефисов
            }
            _Data.linesCount++;
            _Data.fileSize = text.Length;
            _Data.Sum();
            _Data.Sort();
        }//Процедура для сканирования всего текста
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            allText = richTextBox1.Text;
        }
    }
}

