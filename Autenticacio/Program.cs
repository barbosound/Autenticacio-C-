using System;
using System.IO;
using System.Security.Cryptography;


namespace Autenticacio
{
    class Program
    {
        const string Passwords = "arxiuUsers.txt";
        static void Main(string[] args)
        {
            String opcio;


            if (!(File.Exists(Passwords)))
            {
                using (File.Create(Passwords))
                {

                }
            }

            do
            {
                Console.WriteLine("MENÚ");
                Console.WriteLine("(1) Alta usuari");
                Console.WriteLine("(2) Autentica usuari");
                Console.WriteLine("(0) Surt");

                opcio = Console.ReadLine();

                switch (opcio)
                {
                    case "1":
                        AltaUsuari();
                        break;
                    case "2":
                        autentica();
                        break;
                }

            } while (opcio != "0");



        }
        static void AltaUsuari()
        {
            string usuari;
            string password = null;
            string hashStr;
            string existeix;
            string[] arxiu;
            byte[] salt;

            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            Console.WriteLine("Introdueix un nom d'usuari: ");

            usuari = Console.ReadLine();

            existeix = getInfo(usuari);

            if (existeix != null)
            {
                arxiu = existeix.Split(',');

                if (arxiu[0] == usuari)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Aquest usuari ja existeix.");
                    Console.WriteLine("");

                }
                

            }
            else
            {
                if (!(usuari.Contains(" ")))
                {
                    Console.WriteLine("Entra el password: ");

                    ConsoleKeyInfo key;

                    do
                    {
                        key = Console.ReadKey(true);

                        // Si no es retrocés o enter llegeix la tecla
                        if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                        {
                            password += key.KeyChar;
                            Console.Write("*");
                        }
                        // Si és retrocés esborrem el darrer *
                        else if (key.Key == ConsoleKey.Backspace)
                        {
                            if (password.Length > 0)
                            {
                                password.Remove(password.Length - 1);
                                Console.Write("\b \b");

                            }

                        }
                    } while (key.Key != ConsoleKey.Enter);
                    Console.Write("\n");

                    hashStr = CreaHash(password, salt);

                    using (StreamWriter file = new StreamWriter(Passwords, true))
                    {
                        file.WriteLine(usuari + "," + Convert.ToBase64String(salt) + "," + hashStr);
                    }

                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("El nom d'usuari no pot contenir espais");
                    Console.WriteLine("");
                }

                
            }


        }
        static string CreaHash(string pass, byte[] salt)
        {
            try
            {
                var pbkdf2 = new Rfc2898DeriveBytes(pass, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hash);
            }
            catch
            {
                Console.WriteLine("Error al calcular hash");
                Console.WriteLine("");
                return null;

            }
        }
        static void autentica()
        {
            string usuari;
            string password = null;
            string info;

            string[] arxiu;

            byte[] saltUsuari;


            Console.WriteLine("Escriu el nom d'usuari: ");
            usuari = Console.ReadLine();

            Console.WriteLine("Escriu la password: ");
            ConsoleKeyInfo key;
            
            do
            {
                key = Console.ReadKey(true);

                // Si no es retrocés o enter llegeix la tecla
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                // Si és retrocés esborrem el darrer *
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Remove(password.Length - 1);
                        Console.Write("\b \b");

                    }

                }
            } while (key.Key != ConsoleKey.Enter);
            Console.Write("\n");

            info = getInfo(usuari);

            if (info != null)
            {
                arxiu = info.Split(',');

                saltUsuari = Convert.FromBase64String(arxiu[1]);

                string hashString = CreaHash(password, saltUsuari);


                if (hashString == arxiu[2])
                {
                    Console.WriteLine("");
                    Console.WriteLine("Usuari autentificat.");
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("Usuari o contrassenya no són correctes.");
                    Console.WriteLine("");
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("L'usuari no existeix!");
                Console.WriteLine("");
            }
            
        }

        static string getInfo(string usuari)
        {
            try
            {
                using (StreamReader lector = new StreamReader(Passwords))
                {
                    while (lector.Peek() > -1)
                    {
                        string linia = lector.ReadLine();
                        string[] user;
                        string userInfo;

                        if (!String.IsNullOrEmpty(linia))
                        {
                            user = linia.Split(',');

                            if (user[0].Equals(usuari))
                            {
                                userInfo = user[0] + ',' + user[1] + ',' + user[2];
                                return userInfo;
                            }
                        }
                    }
                }
                return null;
            }
            
            catch
            {
                Console.WriteLine("NO s'ha pogut llegir el arxiu");
                Console.WriteLine("");
                return null;
            }
                
            }
        }
        

    }
    

