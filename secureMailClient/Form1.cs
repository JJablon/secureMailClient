using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;


namespace secureMailClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tabControl1.SelectedIndexChanged += new EventHandler(tabControl1_SelectedIndexChanged);

        }
        bool logged = false;
        SmtpClient sc;
        AES_RSA crypt;

        private void button1_Click(object sender, EventArgs e)
        {
            //sc = new SmtpClient(textBox3.Text, Int32.Parse(textBox2.Text));
            //////////////////////////////////
            sc = new SmtpClient(textBox3.Text);
            //   sc.Port = Int32.Parse(textBox2.Text);
            // sc.Host = textBox3.Text;
            sc.EnableSsl = checkBox2.Checked;
            //sc.EnableSsl = false;  ///////////////////////////////////////////////////////
            sc.Timeout = Int32.Parse(textBox9.Text);
            //sc.SendCompleted += new SendCompletedEventHandler(SendCompleted);
            //  sc.UseDefaultCredentials = true;

            //sc.DeliveryMethod = SmtpDeliveryMethod.Network;
            tabControl1.SelectedIndexChanged -= new EventHandler(tabControl1_SelectedIndexChanged);
            logged = true;


        }
        private void ar(object sender, EventArgs ea)
        {

        }



        void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }
        OpenFileDialog opf;
        SaveFileDialog sfd;

        private void Form1_Load(object sender, EventArgs e)
        {
            point:
            opf = new OpenFileDialog();
            opf.DefaultExt = "xml";
            opf.Filter = "Pliki XML|*.xml";
            opf.Multiselect = false;
            opf.Title = "Skąd załadować klucze?";
            DialogResult dr = opf.ShowDialog();
            if (dr == DialogResult.OK && opf.FileName.Contains(".xml") && File.Exists(opf.FileName))
               try
               {
                    crypt = new AES_RSA( opf.FileName,"");//ścieżka do pliku z kluczami to opf.FileName, pusta ścieżka oznacza, że klucza nie zapisujemy
                }
               catch (Exception)
                {
                    MessageBox.Show("Wybierz poprawny plik.Program zostanie zamknięty.");
                    goto point;
                }

            else if (dr == DialogResult.Cancel) {
                sfd = new SaveFileDialog();
                sfd.DefaultExt = "xml";
                sfd.Filter = "Pliki XML|*.xml";
                sfd.Title = "Dokąd wygenerować klucze?";
                DialogResult dresult = sfd.ShowDialog();
                if (dresult == DialogResult.OK)
                    crypt = new AES_RSA("", sfd.FileName); //pusta ścieżka oznacza, że konstruktor klasy powinien wygenerować nowe klucze
                else
                    Application.Exit();
            }
            else
            {
                MessageBox.Show("Wybierz poprawny plik.");
                goto point;
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
      
        private void button3_Click(object sender, EventArgs e)
        {

            try
            {
                MailMessage mm;
                //if (textBox10.Text != "")
                   // mm = new MailMessage(this.textBox1.Text + "@" + textBox10.Text, this.textBox7.Text);
               // else
                    mm = new MailMessage(this.textBox1.Text, this.textBox7.Text);

               
                richTextBox1.Text = textBox6.Text;
                textBox6.Text = "<message>" + crypt.Encrypt(textBox6.Text, maskedTextBox2.Text) + "</message>";
                
                mm.Subject = textBox8.Text;
                mm.SubjectEncoding = Encoding.UTF8;
                mm.Body = textBox6.Text;
                mm.BodyEncoding = Encoding.ASCII;

                try
                {
                    sc.Send(mm);

                }
                catch (SmtpException ex)
                {
                    MessageBox.Show("Timeout: err code: \n" + ex.Data + "\n" + ex.Message + "\n" + ex.StatusCode + "\n" + ex.Source);
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Wysyłanie innej wiadomości prawdopodobnie jeszcze trwa. Spróbuj później");
            }



        }

        private void button4_Click(object sender, EventArgs e)
        {
            sc.SendAsyncCancel();
        }

        private void tabControl1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1 && logged == true)
            {

                try
                {
                    Pop3 obj = new Pop3();
                    obj.Connect(textBox5.Text, textBox1.Text, maskedTextBox1.Text);
                    ArrayList list = obj.List();
                    treeView1.Nodes.Clear();
                    foreach (Pop3Message msg in list)
                    {

                        Pop3Message msg2 = obj.Retrieve(msg);
                        // System.Console.WriteLine("Message {0}: {1}",
                        treeView1.Nodes.Add(msg2.number.ToString());
                        Random r = new Random();
                        treeView1.Nodes[treeView1.Nodes.Count - 1].ForeColor = Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
                        treeView1.Nodes[treeView1.Nodes.Count - 1].Nodes.Add(msg2.message.ToString());
                        //  msg2.number, msg2.message);
                    }
                    obj.Disconnect();
                }
                catch (Pop3Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        byte a = 0x0;
        private void button5_Click(object sender, EventArgs e)
        {
            
            // Rijndael r = new Rijndael();
            //Rijndael.compute(new byte[]{0x32,0x88,0x31,0xe0,0x43,0x5a,0x31,0x37,0xf6,0x30,0x98,0x07,0xa8,0x8d,0xa2,0x34},
            //  new byte[]{0x2b,0x28,0xab,0x09,0x7e,0xae,0xf7,0xcf,0x15,0xd2,0x15,0x4f,0x16,0xa6,0x88,0x3c});

            // string s = Rijndael.cipher("","j");


            // MessageBox.Show(r.mixColumns(new byte[] { 0xd4, 0xe0, 0xb8, 0x1e, 0xbf, 0xb4, 0x41, 0x27, 0x5d, 0x52, 0x11, 0x98, 0x30, 0xae, 0xf1, 0xe5 })[0].ToString());
            //MessageBox.Show(r.mult(0x53, 0xCA).ToString());


            // MessageBox.Show(r.byte_to_bool(r.xor(0xf8, 0x3f)).ToString());
            a++;
            // r.cipher(
            //r.shiftRows(new byte[] { 1, 2, 3, 4,5,6,7,8,9,10,11,12 });
            /*  foreach (byte b in r.shiftRows(new byte[]{1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16}))
              {
                  if (b == true)
                      label13.Text += '1'
                          ;
                  else
                      label13.Text += '0';
                  MessageBox.Show(b.ToString());
              }*/
            //byte b = 0xcc;
            // MessageBox.Show((Bit.byte_to_dec(b))[0].ToString() + " \n" + (Bit.byte_to_dec(b))[1].ToString());
            // MessageBox.Show(r.subBytes(0xa0).ToString());
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            TreeView tr = (TreeView)sender;
            if (tr.SelectedNode != null)
            {
                string raw_message = tr.SelectedNode.Text;
                int output = 0;
                //jeśli klikniemy na korzeń, który jest numerem (integer) wiadomości w skrzynce, to nic się nie stanie
                if (!Int32.TryParse(raw_message, out output))
                {
                    //usunięcie znaków nowej linii
                    raw_message = raw_message.Replace("=", "");
                    raw_message = raw_message.Replace("\n", "");
                    raw_message = raw_message.Replace("\r", "");
                    //MessageBox.Show(tr.SelectedNode.Text);
                    try
                    {
                        int a = raw_message.IndexOf("<message>") + 9;
                        int b = raw_message.IndexOf("</message>");
                        string message = raw_message.Substring(a);
                        message = message.Remove(message.Length - 11, 11);
                        string encrypted = crypt.Decrypt(maskedTextBox3.Text, message, checkBox5.Checked, true);
                        if (!checkBox5.Checked) MessageBox.Show(encrypted);
                    }
                    catch (Exception) { MessageBox.Show("Ta wiadomość ma zły format do odszyfrowania", "To nie jest kryptogram"); }

                }

            }
            tr.SelectedNode = null;
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            
           
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try {
                if (sfd.FileName != "") { File.Delete(sfd.FileName); }
                else if (opf.FileName != "") { File.Delete(opf.FileName); }
            }
            catch (Exception)
            {
                MessageBox.Show("Nie udało się usunac kluczy");
            }

        }
    }























    /// <summary>
    /// klasa zawierająca zestaw metod do szyfrowania mechanizmami AES i RSA
    /// </summary>
    public class AES_RSA
    {
        /// <summary>
        ///  zmienna przechowująca dostawcę usługi kryptograficznej
        /// </summary>
        RSACryptoServiceProvider rsa;

        /// <summary>
        /// zmienna przechowująca klucz publiczny
        /// </summary>
        private string pubkey = "";

        /// <summary>
        /// metoda aktualizująca zmienną pubKey i zwracająca wartość klucza publicznego 
        /// </summary>
        /// <returns>string - odpowiadający prywatnej zmiennej pubKey</returns>
        public string getPubKey()
        {
            pubkey = rsa.ToXmlString(false);
            
            return pubkey;
        }
        /// <summary>
        /// metoda zapisująca klucze RSA do pliku
        /// </summary>
        /// <param name="path">ścieżka do pliku</param>
        public void saveKeys(string path)
        {
            File.WriteAllText(path, rsa.ToXmlString(true));

        }
        /// <summary>
        /// metoda importująca klucze z pliku o danej ścieżce
        /// </summary>
        /// <param name="path"></param>
        private void ImportKeys(string path)
        {

            rsa.FromXmlString(File.ReadAllText(path));

        }

       // string xmlpath = "";
       /// <summary>
       /// publiczny konstruktor klasy
       /// </summary>
       /// <param name="path_from">ścieżka pliku zawierającago klucz do wczytania, jeśli w użyciu - wymaga podania drugiego parametru o wartości ""</param>
       /// <param name="path_to">podanie tego parametru powoduje generację losowych kluczy i zapis pod tę ścieżkę </param>
        public AES_RSA(string path_from="",string path_to="")
        {
            rsa = new RSACryptoServiceProvider();//utworzenie nowych, losowych kluczy
            rsa.PersistKeyInCsp = true;
            if (path_from != "")//jesli podana jest ścieżka z kluczami, to następuje zastąpienie losowych kluczy tymi z pliku
                ImportKeys(path_from);
            else if(path_to != "")
            {
                saveKeys(path_to);
            }
            else
            {
                throw new CryptographicUnexpectedOperationException();
            }
        }
        
        /// <summary>
        /// metoda zwraca plaintext na podstawie szyfrogramu w postaci bajtów
        /// </summary>
        /// <param name="input">tablica bajtów kryptogramu</param>
        /// <returns></returns>
        private string DecryptData(byte[] input)
        {
            string result = "";
            if (rsa == null)
                MessageBox.Show("Klucz nie został ustanowiony");
            else
            {
                //utworzenie instancji klasy odpowiadającej za szyfr symetryczny
                RijndaelManaged rjndl = new RijndaelManaged();
                rjndl.KeySize = 256;
                rjndl.BlockSize = 256;
                rjndl.Mode = CipherMode.CBC;

                //tworzy tablice bajtów przechowujące wielkość użytego klucza i IV
                //te dwie wartości, każda po 4 bajty, są dołączane na początek kryptogramu
                byte[] LenK = new byte[4];
                byte[] LenIV = new byte[4];

                //użycie memorystreama inFs jako strumienia wejściowego danych do zaszyfrowania
                {
                    MemoryStream inFs = new MemoryStream(input);
                    inFs.Seek(0, SeekOrigin.Begin);
                    inFs.Seek(0, SeekOrigin.Begin);
                    inFs.Read(LenK, 0, 3);
                    inFs.Seek(4, SeekOrigin.Begin);
                    inFs.Read(LenIV, 0, 3);

                    //zamiana wartości zapisanych w tablicach bajtów na liczbę całkowitą
                    int lenK = BitConverter.ToInt32(LenK, 0);
                    int lenIV = BitConverter.ToInt32(LenIV, 0);


                    //ustalenie pozycji szyfrogramu (startC) i jego długości
                    int startC = lenK + lenIV + 8;
                    int lenC = (int)inFs.Length - startC;


                    //utworzenie tablicy bajtów na zaszyfrowany RSA klucz AESa, IV i szyfrogram
                    byte[] KeyEncrypted = new byte[lenK];
                    byte[] IV = new byte[lenIV];


                    //rozpakowanie klucza i IV-a, który startuje na 8 bajcie (zaraz po 2x4 bajtach długości)
                    inFs.Seek(8, SeekOrigin.Begin);
                    inFs.Read(KeyEncrypted, 0, lenK);
                    inFs.Seek(8 + lenK, SeekOrigin.Begin);
                    inFs.Read(IV, 0, lenIV);

                    //użycie RSA do odszyfrowania klucza AESA
                    byte[] KeyDecrypted = rsa.Decrypt(KeyEncrypted, false);

                    //utworzenie dekryptora AES na podstawie odzyskanego klucza
                    ICryptoTransform transform = rjndl.CreateDecryptor(KeyDecrypted, IV);

                    //odszyfrowanie szyfrogramu z memorystreama 
                    using (MemoryStream outFs = new MemoryStream())
                    {

                        int count = 0;
                        int offset = 0;

                        //poniższa zmienna może mieć dowolny ustalony rozmiar:
                        int blockSizeBytes = rjndl.BlockSize / 8;
                        byte[] data = new byte[blockSizeBytes];



                        //przewiń na początek szyfrogramu
                        inFs.Seek(startC, SeekOrigin.Begin);
                        //z użyciem bezpiecznego strumienia odszyfruj kryptogram (z outFs) i wpisz do outStream
                        using (CryptoStream outStreamDecrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                        {
                            do
                            {
                                count = inFs.Read(data, 0, blockSizeBytes);
                                offset += count;
                                outStreamDecrypted.Write(data, 0, count);

                            }
                            while (count > 0);
                            //zamknięcie strumienia
                            outStreamDecrypted.FlushFinalBlock();
                            outStreamDecrypted.Close();
                        }
                        //przekształcenie bajtów na Unicode
                        result = Encoding.Unicode.GetString(outFs.ToArray());
                        //zamknięcie strumieni
                        outFs.Close();
                    }
                    inFs.Close();

                }
            }
            return result;

        }
        /// <summary>
        /// zwraca tablicę bajtów odpowiadającą kryptogramowi tworzonemu na bazie parametru input
        /// </summary>
        /// <param name="data_in">dane wejściowe do zaszyfrowania</param>
        /// <returns></returns>
        private byte[] EncryptData(string data_in)
        {
            byte[] result = { };
            if (rsa == null)
                MessageBox.Show("Klucz nie jest ustawiony");
            else
            {
                //utworzenie instancji klasy dostarczającej usługę szyfrowania algorytmem AES
                RijndaelManaged rjndl = new RijndaelManaged();
                //ustawienie rozmiaru klucza na 256 bit
                rjndl.KeySize = 256;
                //ustawienie rozmiaru szyfrowanego bloku na 256 bit
                rjndl.BlockSize = 256;
                rjndl.Mode = CipherMode.CBC;

                //utworzenie szyfratora
                ICryptoTransform transform = rjndl.CreateEncryptor();

                //użycie RSA w celu zaszyfrowania klucza AES
                byte[] keyEncrypted = rsa.Encrypt(rjndl.Key, false);

                //tablice bajtów zawierające rozmiar klucza i IVa
                byte[] LenK = new byte[4];
                byte[] LenIV = new byte[4];

                //przekształcenie danych zapisanych w powyższych tablicach do 
                int lKey = keyEncrypted.Length;
                LenK = BitConverter.GetBytes(lKey);
                int lIV = rjndl.IV.Length;
                LenIV = BitConverter.GetBytes(lIV);


                //zapisanie do MemoryStreama:
                MemoryStream outFs = new MemoryStream();
                outFs.Write(LenK, 0, 4);                // - długości klucza
                outFs.Write(LenIV, 0, 4);               // - długości IVa
                outFs.Write(keyEncrypted, 0, lKey);     // - zaszyfrowanego klucza Rijn
                outFs.Write(rjndl.IV, 0, lIV);          // zaszyfrowanego IVa

                //zapis szyfrogramu do strumienia wyjściowego z użyciem bezpiecznej klasy CryptoStream
                using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                {

                    int count = 0;
                    int offset = 0;
                    int blockSizeBytes = rjndl.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];
                    int bytesRead = 0;
                    //zamiana znaków ciągu szyfrowanego na bajty zgodnie z Unicode
                    byte[] bytes = Encoding.Unicode.GetBytes(data_in);
                    //wczytanie bajtów do memorystreama
                    using (MemoryStream inFs = new MemoryStream(bytes))
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamEncrypted.Write(data, 0, count);
                            bytesRead += blockSizeBytes;
                        }
                        while (count > 0);
                        inFs.Close();
                    }
                    //zamknięcie strumieni ze względu na stabilność i bezpieczeństwo
                    outStreamEncrypted.FlushFinalBlock();
                    outStreamEncrypted.Close();
                }
                result = outFs.ToArray();

                outFs.Close();
                //   }

            }
            return result;
        }

        /// <summary>
        /// zmienna testowa
        /// </summary>
        private byte[] bytes;
        /// <summary>
        /// zmienna trzymająca ścieżkę do pliku testowego
        /// </summary>
      //  private string pathnew = "C:\\crypto\\crypto.csv";
        /// <summary>
        /// metoda testowa
        /// </summary>
        public string Encrypt(string message, string signature, bool save_to_file = false, string pathnew = "")
        {
            if (save_to_file && File.Exists(pathnew) == false) File.Create(pathnew);
            //CreateKeys();
            getPubKey();
            DateTime dt = DateTime.Now;
            try
            { //bytes = EncryptFile("ąźćńóęłś");
                bytes = EncryptData(message);
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Nie udało się zaszyfrować.");
            }
            string s = "";
            foreach (byte b in bytes)
            {
                s += b.ToString();
                s += ',';
            }
            s += '|';
            MD5 md = MD5.Create();

            byte[] checksum = rsa.Encrypt(md.ComputeHash(Encoding.Unicode.GetBytes(message), 0, Encoding.Unicode.GetBytes(message).Length), false);
            byte[] signature_byte = rsa.Encrypt(md.ComputeHash(Encoding.Unicode.GetBytes(signature), 0, Encoding.Unicode.GetBytes(signature).Length), false);
            foreach (byte b in checksum)
            {
                s += b.ToString();
                s += ',';
            }
            s += '|';
            foreach (byte b in signature_byte)
            {
                s += b.ToString();
                s += ',';
            }
            if (save_to_file) File.WriteAllText(pathnew, s);
            return s;

        }
        /// <summary>
        /// metoda testowa
        /// </summary>
        public string Decrypt(string signature_s, string content = "", bool show_popups = true, bool check_signature = true, bool read_from_file = false, string pathnew = "")
        {
           // CreateKeys();
            //GetPrivateKey();


            try
            {
                //wczytanie pliku CSV
                if (read_from_file) content = File.ReadAllText(pathnew);
                //rozdzielenie przecinków i części z podpisem
                string encrypted = content.Split('|')[0];
                string checksum = content.Split('|')[1];
                string signature = content.Split('|')[2];
                string[] data = encrypted.Split(',');
                string[] checksum_splitted = checksum.Split(',');
                string[] signature_splitted = signature.Split(',');
                //inicjalizacja pomocniczych list bajtów, celem łatwych operacji i późniejszej konwersji do byte[]
                List<byte> ls = new List<byte>();
                List<byte> ls1 = new List<byte>();
                List<byte> ls2 = new List<byte>();

                //tworzenie list bajtów: z wiadomością, sumą kontrolną i podpisem 
                foreach (string s in data) if (s != "") ls.Add(byte.Parse(s));
                foreach (string s in checksum_splitted) if (s != "") ls1.Add(byte.Parse(s));
                foreach (string s in signature_splitted) if (s != "") ls2.Add(byte.Parse(s));
                //DateTime dt = DateTime.Now;

                //konwersja listy na tablicę bajtów i deszyfrowanie
                string decrypted = DecryptData(ls.ToArray());
                //utworzenie obiektu generującego skrót do porównania ze skrótem odszyfrowanym z wiadomości
                MD5 md = MD5.Create();
                //obliczenie skrótu na podstawie odszyfrowanego kryptogramu
                byte[] bytes = md.ComputeHash(Encoding.Unicode.GetBytes(decrypted));
                //utworzenie tablicy bajtów na podstawie skrótu przesłanego z wiadomością
                byte[] bytes1 = rsa.Decrypt(ls1.ToArray(), false);
                //utworzenie tablic bajtów z podpisem do porównania zgodności
                byte[] bytes_of_signature = md.ComputeHash(Encoding.Unicode.GetBytes(signature_s));
                byte[] bytes_of_signature2 = rsa.Decrypt(ls2.ToArray(), false);
                if (show_popups) MessageBox.Show(decrypted);

                //flaga ustawiana na false jeśli dowolny bit sumy kontrolnej uzyskanej na podstawie odszyfrowanej wiadomości jest niezgodny z sumą kontrolną "doklejoną" do wiadomości
                bool flag_of_checksum = true;
                for (int a = 0; a < bytes.Length && a < bytes1.Length; a++)
                {
                    if (bytes[a] != bytes1[a]) flag_of_checksum = false;
                }

                //flaga ustawiana na false jeśli dowolny bit sumy kontrolnej uzyskanej na podstawie odszyfrowanej wiadomości jest niezgodny z sumą kontrolną "doklejoną" do wiadomości
                bool flag_of_signature = true;
                for (int a = 0; a < bytes_of_signature.Length && a < bytes_of_signature2.Length; a++)
                {
                    if (bytes_of_signature[a] != bytes_of_signature2[a]) flag_of_signature = false;
                }

                if (flag_of_checksum == true)
                {
                    if (show_popups) MessageBox.Show("Test sumy kontrolnej: poprawny");
                    if (flag_of_signature == true)
                    {
                        if (show_popups) MessageBox.Show("Test podpisu: poprawny");
                        else
                            MessageBox.Show("NIEPOPRAWNY PODPIS!", "UWAGA");
                        return decrypted;
                    }
                    else
                        return "ERROR";

                }
                else
                    return "ERROR";
                // MessageBox.Show((DateTime.Now - dt).TotalMilliseconds.ToString());
            }
            catch (CryptographicException)
            {
                if (show_popups) MessageBox.Show("Nie udało się odszyfrować. Sprawdź nazwę kontenera.");
                return "ERROR";
            }




        }
    }

























public class Pop3Exception : System. ApplicationException 
{ 
        /// <summary>
        /// publiczny konstruktor klasy wyjątku rzucanego w razie błędu w module POP3
        /// </summary>
        /// <param name="str">opis błędu</param>
    public Pop3Exception( string str) 
        : base( str) 
    { 
    } 
}
        public class Pop3Message 
{ 

    public long number; 
    public long bytes; 
    public bool retrieved; 
    public string message; 
}


       /// <summary>
       /// klasa odpowiedzialna za obsługę protokołu pocztowego POP3
       /// </summary>
       public class Pop3 : System.Net.Sockets.TcpClient 
        {
        /// <summary>
        /// medota odbierająca wiadomość z serwera
        /// </summary>
        /// <param name="rhs">otrzymana wiadomość</param>
        /// <returns></returns>
            public Pop3Message Retrieve(Pop3Message rhs)
            {
                string message;
                string response;

                Pop3Message msg = new Pop3Message();
                msg.bytes = rhs.bytes;
                msg.number = rhs.number;

                message = "RETR " + rhs.number + "\r\n";
                Write(message);
                response = Response();
                if (response.Substring(0, 3) != "+OK")
                {
                    throw new Pop3Exception(response);
                }

                msg.retrieved = true;
                while (true)
                {
                    response = Response();
                    if (response == ".\r\n")
                    {
                        break;
                    }
                    else
                    {
                        msg.message += response;
                    }
                }

                return msg;
            }


            public ArrayList List()
            {
                string message;
                string response;

                ArrayList retval = new ArrayList();
                message = "LIST\r\n";
                Write(message);
                response = Response();
                if (response.Substring(0, 3) != "+OK")
                {
                    throw new Pop3Exception(response);
                }
                
                while (true)
                {
                    response = Response();
                    if (response == ".\r\n")
                    {
                        return retval;
                    }
                    else
                    {
                        Pop3Message msg = new Pop3Message();
                        char[] seps = { ' ' };
                        string[] values = response.Split(seps);
                        msg.number = Int32.Parse(values[0]);
                        msg.bytes = Int32.Parse(values[1]);
                        msg.retrieved = false;
                        retval.Add(msg);
                        continue;
                    }
                }
            }

            /// <summary>
            /// metoda nawiązująca połączenie z serwerem POP3
            /// </summary>
            /// <param name="server">adres hosta POP3</param>
            /// <param name="username">nazwa użytkownika</param>
            /// <param name="password">hasło użytkownika</param>
            public void Connect(string server, string username, string password)
            {
                string message;
                string response;
                //if(
                     

                try
                {
                    
                    Connect(server, 110);
                }
                catch (Exception)
                {
                    MessageBox.Show("err");
                }
                response = Response();
                if (response.Substring(0, 3) != "+OK")
                {
                    throw new Pop3Exception(response);
                }

                message = "USER " + username + "\r\n";
                Write(message);
                response = Response();
                if (response.Substring(0, 3) != "+OK")
                {
                    throw new Pop3Exception(response);
                }

                message = "PASS " + password + "\r\n";
                Write(message);
                response = Response();
                if (response.Substring(0, 3) != "+OK")
                {
                    throw new Pop3Exception(response);
                }
            }
            /// <summary>
            /// metoda rozłączająca połączenie z serwerem
            /// </summary>
            public void Disconnect()
            {
                string message;
                string response;
                message = "QUIT\r\n";
                Write(message);
                response = Response();
                if (response.Substring(0, 3) != "+OK")
                {
                    throw new Pop3Exception(response);
                }
            }
        /// <summary>
        /// metoda wypisująca na standardowe wyjście daną wiadomość w kodowaniu ASCII
        /// </summary>
        /// <param name="message"></param>
            private void Write(string message)
            {
                System.Text.ASCIIEncoding en = new System.Text.ASCIIEncoding();

                byte[] WriteBuffer = new byte[1024];
                WriteBuffer = en.GetBytes(message);

                NetworkStream stream = GetStream();
                stream.Write(WriteBuffer, 0, WriteBuffer.Length);

                Debug.WriteLine("WRITE:" + message);
            }
      
            private string Response()
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] serverbuff = new Byte[1024];
                NetworkStream stream = GetStream();
                int count = 0;
                while (true)
                {
                    byte[] buff = new Byte[2];
                    int bytes = stream.Read(buff, 0, 1);
                    if (bytes == 1)
                    {
                        serverbuff[count] = buff[0];
                        count++;

                        if (buff[0] == '\n')
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    };
                };

                string retval = enc.GetString(serverbuff, 0, count);
                Debug.WriteLine("READ:" + retval);
                return retval;
            }
        }



    //nieużywany kod z pierwszego etapu projektu
    /*
       public static class Rijndael
       {
          // private string data;
           
           public static string  cipher(string data, string key)
           {
               List<byte> bytes = new List<byte>(); 
                byte []dat = new byte[16];
               byte [] _key = new byte[16];
               string result="";
               //string key2;
               //this.data = data;
               int counter = 0,counter2 = 0;
               //if (key.Length < 16) //przedłużenie hasła tak, aby klucz miał 128 bit
               //{
                   for (int a = 0; a < 16; a++)
                   {
                       //key2 += key[a % key.Length];
                       char c = key[a % key.Length];
                       _key[a] = (byte)c;
                   }
              // }
             //  key2 = key2.Substring(0, 16); //ograniczenie długości klucza do 128 bit

               if(false ) throw new ArgumentException();
               while (data.Length %16 != 0||data.Length<16) data = data.Insert(data.Length - 1, " ");
               foreach (char c in data)
               {
                   
                   if (counter < 16)
                       dat[counter] = (byte)c;
                   else {  //co 16 znaków (128 bitów) przekaż blok danych do funkcji szyfrującej
                       counter = 0;
                       bytes.AddRange(compute(dat, _key));
                       dat = new byte[16]; 
                   }
                   counter++;
               }

               foreach (byte b in bytes)
               {
                   result += (char)b;

               }
               return result;


           }
           public static byte[] compute(byte[] input,byte[] key)
           {
               //byte[] block = init_perm(input); //permutacja początkowa
               byte[] keys = keyGenerate(key);
               byte[] block = addRoundKey(input, key); 
               byte[] rounded = new byte[16];
               byte[] shifted = new byte[16];
               byte[] boxed = new byte[16];
               byte[] mixed = new byte[16];
               for (int no=0;no<9;no++){//problem w rundzie 2 w shifted
                   for (int a = 0; a < 16; a++) key[a] = keys[(no+1) * 16 + a];
                    rounded = new byte[16];
                   shifted = new byte[16];
                   boxed = new byte[16];
                    mixed = new byte[16];
                    
                   boxed = subBytes(block);
                   shifted = shiftRows(boxed);
                   //uwaga! od nastepnego wiersza wynik jest czytany KOLUMNAMI
                   mixed = mixColumns(shifted);
                   // usunac to!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                   //key = new byte[] { 0xa0, 0x88, 0x23, 0x2a, 0xfa, 0x54, 0xa3, 0x6c, 0xfe, 0x2c, 0x39, 0x76, 0x17, 0xb1, 0x39, 0x05 };
                   
                   //tutaj: generacja klucza nast rundy
                   
                   //transpozycja
                   //mixed = transp(mixed);
                   
                   rounded = addRoundKey(mixed, key);
                   block = rounded;
                   block = transp(block);
               }

               //osttnia runda - bez mixa
                   boxed = subBytes(block);
                   shifted = shiftRows(boxed);
                  // mixed = mixColumns(shifted);
                   mixed = shifted; //brak mixu kolumn
                   for (int a = 0; a < 16; a++) key[a] = keys[10 * 16 + a];
                   mixed = transp(mixed);
                   rounded = addRoundKey(mixed, key);
               



               return rounded;
           }
           private static byte[][] rconstant = new byte[10][];
           public static byte[] keyGenerate(byte[] initkey)
          {
               rconstant[0] = new byte []{0x1,0,0,0};
               rconstant[1] = new byte[] { 0x2, 0, 0, 0 };
               rconstant[2] = new byte[] { 0x4, 0, 0, 0 };
               rconstant[3] = new byte[] { 0x8, 0, 0, 0 };
               rconstant[4] = new byte[] { 0x10, 0, 0, 0 };
               rconstant[5] = new byte[] { 0x20, 0, 0, 0 };
               rconstant[6] = new byte[] { 0x40, 0, 0, 0 };
               rconstant[7] = new byte[] { 0x80, 0, 0, 0 };
               rconstant[8] = new byte[] { 0x1b, 0, 0, 0 };
               rconstant[9] = new byte[] { 0x36, 0, 0, 0 };
               int nr_round = 0, cnt = 0;
              byte[] keys = new byte[180];
               byte[] copy = new byte[16];
               initkey.CopyTo(copy,0);
               copy = transp(copy);
              copy.CopyTo(keys, 0);


              for (int b = 0; b < 40; b++)
              {
                  //byte[] col = new byte[] { keys[cnt], keys[cnt + 4], keys[cnt + 8], keys[cnt + 12] };
                  byte[] col_res = new byte[4];
                  //byte[] col2 = new byte[] { keys[cnt + 7], keys[cnt + 11], keys[cnt + 15], keys[cnt + 3] };
                  byte[] col = new byte[] { keys[cnt], keys[cnt + 1], keys[cnt + 2], keys[cnt + 3] };
                  byte[] col2;
                  bool special = false;

                  if(cnt==0||cnt%16==0) special = true;
                    if(special)  col2 = new byte[] { keys[cnt + 13], keys[cnt + 14], keys[cnt + 15], keys[cnt + 12] };
                  else col2 = new byte[] { keys[cnt + 12], keys[cnt + 13], keys[cnt + 14], keys[cnt + 15] };
                
                  for(int c=0;c<4;c++)
                  if(special) col2[c] = subByte(col2[c]);

                  for (int a = 0; a < 4; a++)
                  {

                      col_res[a] = Bit.xor(col[a], col2[a]);
                      ///dodac wiecej warunkow tu:
                      if (special)
                      {
                          col_res[a] = Bit.xor(col_res[a], rconstant[nr_round][a]);
                          
                      }
                      keys[16+ cnt + a] = col_res[a];
                  }
                  if (special) nr_round++;
                  
                  cnt += 4;
              }
              return keys;
               
          }



           public static byte mult(byte x, byte y)
           {
               byte result = 0,a = x, b = y;
               while (b!=0)
               {
                   if (b%2==1)  result = Bit.xor(result,a);
                      if (a >=128) a = Bit.xor_int(a << 1, 0x11b);
                      else
                          a <<= 1;
                      b >>= 1;
                   

               }
               return result;
           }
           public static byte[] transp(byte[] input)
           {
              
               byte[] output = new byte[16];
               for (int a = 0; a < 4; a++)
               {
                   output[a*4+0] = input[a];
                   output[a * 4 + 1] = input[4 + a];
                   output[a * 4 + 2] = input[8 + a];
                   output[a * 4 + 3] = input[12 + a];
               }
               return output;

           }
           public static byte[] mixColumns(byte[] input)
           {
               byte[] output = new byte[16];
               for (int a = 0; a < 4; a++)
               {
                   //int a = 2;
                   byte[] row = new byte[] { input[a], input[a + 4], input[a + 8], input[a + 12] };


                   a *= 4;
                   output[a+0] = Bit.xor(mult(2, row[0]),mult(3, row[1]));
                   output[a+0] = Bit.xor(output[a+0], row[2]);
                   output[a+0] = Bit.xor(output[a+0], row[3]);
                   output[a+1] = Bit.xor(row[0], mult(2, row[1]));
                   output[a+1] = Bit.xor(output[a+1], mult(3,row[2]));
                   output[a+1] = Bit.xor(output[a+1], row[3]);
                   output[a+2] = Bit.xor(row[0],  row[1]);
                   output[a+2] = Bit.xor(output[a+2], mult(2, row[2]));
                   output[a+2] = Bit.xor(output[a+2], mult(3,row[3]));
                   output[a+3] = Bit.xor(mult(3,row[0]), row[1]);
                   output[a+3] = Bit.xor(output[a+3],  row[2]);
                   output[a+3] = Bit.xor(output[a+3], mult(2,row[3]));
                   a /= 4;

               }
               
                   return output;

           }
           public static byte[] subBytes(byte[] input) //spr
           {
               byte[] output = new byte[16];
                   int counter = 0;
               foreach (byte b in input)
               {
                   output[counter] = subByte(b);
                   counter++;
               }
               return output;

           }
           public static byte subByte(byte b) //spr
           {
               int[] val = Bit.byte_to_dec(b);
               int index = 16 * val[0] + val[1];
               return subs[index];
           }
           public static byte[] shiftRows(byte[] input)
           {
               byte[] row1 = new byte[] { input[0], input[1], input[2], input[3] }; //0 1 2 3
               byte[] row2 = new byte[] { input[5], input[6], input[7], input[4] }; //1 2 3 0
               byte[] row3 = new byte[] { input[10], input[11], input[8], input[9] };//2 3 0 1
               byte[] row4 = new byte[] { input[15], input[12], input[13], input[14] };
              byte[] rows = new byte[16];
               row1.CopyTo(rows,0);
               row2.CopyTo(rows,4);
               row3.CopyTo(rows,8);
               row4.CopyTo(rows,12);

               return rows;

           }
           private static byte[] addRoundKey(byte[] input, byte[] key)
           {
               byte[] output = new byte[16];
               int counter = 0;
               foreach(byte b in input){
                   output[counter] = Bit.xor(b,  key[counter]);
                   counter++;
           }
               return output;
           }
          
           
           private static byte[] init_perm(byte[] input)
           {
               byte[] output = new byte[16];
               if (input.Length == 16)
               {

                   return new byte[] { input[0], input[4], input[8], input[12], input[1], input[5], input[9], input[13], input[2], input[6], input[10], input[14], input[3], input[7], input[11], input[15] };
               }
               return null;
           }
















          static byte[] subs = new byte[] 
 {
    0x63, 0x7C, 0x77, 0x7B, 0xF2, 0x6B, 0x6F, 0xC5, 0x30, 0x01, 0x67, 0x2B, 0xFE, 0xD7, 0xAB, 0x76,
    0xCA, 0x82, 0xC9, 0x7D, 0xFA, 0x59, 0x47, 0xF0, 0xAD, 0xD4, 0xA2, 0xAF, 0x9C, 0xA4, 0x72, 0xC0,
    0xB7, 0xFD, 0x93, 0x26, 0x36, 0x3F, 0xF7, 0xCC, 0x34, 0xA5, 0xE5, 0xF1, 0x71, 0xD8, 0x31, 0x15,
    0x04, 0xC7, 0x23, 0xC3, 0x18, 0x96, 0x05, 0x9A, 0x07, 0x12, 0x80, 0xE2, 0xEB, 0x27, 0xB2, 0x75,
    0x09, 0x83, 0x2C, 0x1A, 0x1B, 0x6E, 0x5A, 0xA0, 0x52, 0x3B, 0xD6, 0xB3, 0x29, 0xE3, 0x2F, 0x84,
    0x53, 0xD1, 0x00, 0xED, 0x20, 0xFC, 0xB1, 0x5B, 0x6A, 0xCB, 0xBE, 0x39, 0x4A, 0x4C, 0x58, 0xCF,
    0xD0, 0xEF, 0xAA, 0xFB, 0x43, 0x4D, 0x33, 0x85, 0x45, 0xF9, 0x02, 0x7F, 0x50, 0x3C, 0x9F, 0xA8,
    0x51, 0xA3, 0x40, 0x8F, 0x92, 0x9D, 0x38, 0xF5, 0xBC, 0xB6, 0xDA, 0x21, 0x10, 0xFF, 0xF3, 0xD2,
    0xCD, 0x0C, 0x13, 0xEC, 0x5F, 0x97, 0x44, 0x17, 0xC4, 0xA7, 0x7E, 0x3D, 0x64, 0x5D, 0x19, 0x73,
    0x60, 0x81, 0x4F, 0xDC, 0x22, 0x2A, 0x90, 0x88, 0x46, 0xEE, 0xB8, 0x14, 0xDE, 0x5E, 0x0B, 0xDB,
    0xE0, 0x32, 0x3A, 0x0A, 0x49, 0x06, 0x24, 0x5C, 0xC2, 0xD3, 0xAC, 0x62, 0x91, 0x95, 0xE4, 0x79,
    0xE7, 0xC8, 0x37, 0x6D, 0x8D, 0xD5, 0x4E, 0xA9, 0x6C, 0x56, 0xF4, 0xEA, 0x65, 0x7A, 0xAE, 0x08,
    0xBA, 0x78, 0x25, 0x2E, 0x1C, 0xA6, 0xB4, 0xC6, 0xE8, 0xDD, 0x74, 0x1F, 0x4B, 0xBD, 0x8B, 0x8A,
    0x70, 0x3E, 0xB5, 0x66, 0x48, 0x03, 0xF6, 0x0E, 0x61, 0x35, 0x57, 0xB9, 0x86, 0xC1, 0x1D, 0x9E,
    0xE1, 0xF8, 0x98, 0x11, 0x69, 0xD9, 0x8E, 0x94, 0x9B, 0x1E, 0x87, 0xE9, 0xCE, 0x55, 0x28, 0xDF,
    0x8C, 0xA1, 0x89, 0x0D, 0xBF, 0xE6, 0x42, 0x68, 0x41, 0x99, 0x2D, 0x0F, 0xB0, 0x54, 0xBB, 0x16
 };

       }
       public static class Bit
       {
           public static bool[] byte_to_bool(byte x)
           {
               bool[] b = new bool[8];
               for (int a = 8; a >= 0; a--)
               {
                   if (((double)x / Math.Pow(2, (double)a)) >= 1) { x -= (byte)Math.Pow(2, (double)a); b[7 - a] = true; }

               }
               return b;
           }
           public static bool[] int_to_bool(int x)
           {
               bool[] b = new bool[9];
               for (int a = 9; a >= 0; a--)
               {
                   if (((double)x / Math.Pow(2, (double)a)) >= 1) { x -= (int)Math.Pow(2, (double)a); b[8 - a] = true; }

               }
               return b;
           }
           public static int[] byte_to_dec(byte x)
           {  
               bool[] b = new bool[8];
               int[] output = new int[2];
               for (int a = 8; a >= 0; a--)
               {
                   if (((double)x / Math.Pow(2, (double)a)) >= 1) { x -= (byte)Math.Pow(2, (double)a); b[7 - a] = true; }

               }
               int first=0, second=0, counter = 0;
               for (int c = 0; c < 4; c++)
               {
                   if (b[c] == true) first += (int)Math.Pow(2, (double)(3 - c));
               }

               for (int c = 4; c < 8; c++)
               {
                   if (b[c] == true) second += (int)Math.Pow(2, (double)(3-counter));
                   counter++;
               }
               
               output[0] = first;
               output[1] = second;



               return output;
           
               
           }
           

           public static byte xor(byte x, byte y) 
           {
               bool[] xx = byte_to_bool(x);
               bool[] yy = byte_to_bool(y);
               int output = 0;
               for (int a = 0; a < 8; a++)
               {
                   if (xx[a] != yy[a]) output += (int)Math.Pow((double)2, (double)(7 - a));// ((int)2 ^ (int)(8 - a));

               }
               return (byte)output;
           }

           public static byte xor_int(int x, int y)
           {
               bool[] xx = int_to_bool(x);
               bool[] yy = int_to_bool(y);
               int output = 0;
               for (int a = 0; a < 9; a++)
               {
                   if (xx[a] != yy[a]) output += (int)Math.Pow((double)2, (double)(8 - a));// ((int)2 ^ (int)(8 - a));

               }
               return (byte)output;
           }




       }

    */
}
