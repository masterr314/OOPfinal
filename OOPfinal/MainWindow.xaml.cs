using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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

namespace OOPfinal
{
    public partial class MainWindow : Window
    {

        public string Salt { get; set; }
        public string FilePath { get; set; }
        public string Hash { get; set; }
        public int Result { get; set; }
        public List<string> items { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.Salt = "sdjk56934hjr3";
            this.FilePath = "password.txt";
            this.Result = 0;
            this.items = new List<string>();
        }

        private void hashBtn_Click(object sender, RoutedEventArgs e)
        {
            string password = this.passwordField.Password;
            string hashedPass = this.hash(password);
            this.Hash = hashedPass;
            MessageBox.Show(hashedPass);
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            TextWriter txt = new StreamWriter(this.FilePath);
            txt.Write(this.Hash);
            txt.Close();
            MessageBox.Show("File succesfully saved!");
        }

        private async void btnStartBrute_Click(object sender, RoutedEventArgs e)
        {
            listAttempts.Items.Clear();
            this.items.Clear();

            Stopwatch watch = Stopwatch.StartNew();

            int threads = Convert.ToInt32(threadField.Text);
            int counter = 0;
            List<Task> tasks = new List<Task>();

            // Characters
            bool isFound = false;
            // 1 Character
            for (char first = 'a'; first <= (int)'z'; first++)
            {
                if (isFound) break;
                if (this.hash(first.ToString()) == this.Hash)
                {
                    string content = $"{first}\tSuccessfull";
                    this.items.Add(content);
                    isFound = true;
                }
                else
                {
                    string content = $"{first}\tFailed";
                    this.items.Add(content);
                }
            }
            // 2 Characters
            if (!isFound)
            {
                for (char first = 'a'; first <= (int)'z'; first++)
                {
                    if (isFound) break;
                    for (char second = 'a'; second <= (int)'z'; second++)
                    {
                        if (isFound) break;
                        if (this.hash(first.ToString() + second) == this.Hash)
                        {
                            string content = $"{first.ToString() + second}\tSuccessfull";
                            this.items.Add(content);
                            isFound = true;
                        }
                        else
                        {
                            string content = $"{first.ToString() + second}\tFailed";
                            this.items.Add(content);
                        }
                    }
                }
            }
            // 3 Characters
            if (!isFound)
            {
                for (char first = 'a'; first <= (int)'z'; first++)
                {
                    if (isFound) break;
                    for (char second = 'a'; second <= (int)'z'; second++)
                    {
                        if (isFound) break;
                        for (char third = 'a'; third <= (int)'z'; third++)
                        {
                            if (isFound) break;
                            if (this.hash(first.ToString() + second + third) == this.Hash)
                            {
                                string content = $"{first.ToString() + second + third}\tSuccessfull";
                                this.items.Add(content);
                                isFound = true;
                            }
                            else
                            {
                                string content = $"{first.ToString() + second + third}\tFailed";
                                this.items.Add(content);
                            }
                        }
                    }
                }
            }
            //

            // Numbers
            //for (int t = 0; t < threads; t++)
            //{
            //    tasks.Add(Task.Factory.StartNew(() =>
            //    {
            //        for (int i = counter; i < 99999; i += threads)
            //        {
            //            if (this.Result == 1) break;
            //            if (this.hash(i.ToString()) == this.Hash)
            //            {
            //                string content = $"{i}\tThread: {t}\tSuccessfull";
            //                Brush background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("LightGreen");
            //                this.items.Add(content);
            //                this.Result = 1;
            //                break;
            //            }
            //            else
            //            {
            //                string content = $"{i}\tThread: {t}\tFailed";
            //                Brush background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Red");
            //                this.items.Add(content);
            //            }
            //        }
            //    }));
            //    counter++;
            //}
            //var finished = await Task.WhenAny(tasks);
            //

            for (int i = 0; i < this.items.Count; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = this.items[i];
                if (this.items[i].Contains("Successfull")) {
                    Brush background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("LightGreen");
                    item.Background = background;
                } else
                {
                    Brush background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Red");
                    item.Background = background;
                }
                this.listAttempts.Items.Add(item);
            }
  
            watch.Stop();
            timeLabel.Content = watch.Elapsed.TotalMilliseconds + "ms";
        }
        
        private string hash(string password)
        {
            byte[] salt = Encoding.ASCII.GetBytes(this.Salt);
            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                // Concatenate password and salt
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                // Hash the concatenated password and salt
                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                // Concatenate the salt and hashed password for storage
                byte[] hashedPasswordWithSalt = new byte[hashedBytes.Length + salt.Length];
                Buffer.BlockCopy(salt, 0, hashedPasswordWithSalt, 0, salt.Length);
                Buffer.BlockCopy(hashedBytes, 0, hashedPasswordWithSalt, salt.Length, hashedBytes.Length);

                return Convert.ToBase64String(hashedPasswordWithSalt);
            }
        }
    }
}
