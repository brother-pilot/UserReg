using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace UserReg
{
    class Program
    {
        static List<FileInfo> files = new List<FileInfo>();
        static string pathModel;
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.WriteLine("Введите первые три буквы ФИО на английском:");
            string fio=Console.ReadLine();
            pathModel = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(pathModel);
            int index = 1;
            ReadFile("Filials.txt")
                .ForEach(x=>Console.WriteLine(index+++" "+x));
            Console.WriteLine("Введение номер, соответствующий филиалу работника:");
            int branch = int.Parse(Console.ReadLine());
            List<string> place = ReadFile("Branch_"+branch+".txt");
            InitializeListFilesForSave(fio, branch, place);
            List<string> storingPlace = ReadFile("StoringPlace.txt");
            WriteFiles(fio, branch, place, storingPlace);
            Console.WriteLine("Введите пароль для пользователя:");
            string pass = Console.ReadLine();
            WriteFileForClientm(fio, pass, place);
        }

        private static void WriteFileForClientm(string fio, string pass, List<string> place)
        {
            string[] str = new string[3*(files.Count+1)/2];
            int index = 0;
            try
            {
                for (int i = 0; i < files.Count; i += 2)
                {
                    str[index++] = place[i + 1] + fio;
                    str[index++] = pass;
                    str[index++]= "C=" + files[i + 1].Name + "; B=" + files[i].Name + "; P=; K=; A=; M=; ";
                }
                File.WriteAllLines("LineForFileClientm.txt", str, Encoding.GetEncoding(1251));
            }
            catch (Exception)
            {
                throw new Exception("Не удалось сохранить данные в файл LineForFileClientm!");
            }
        }

        public static List<String> ReadFile(string fileName)
        {
            var directory = new DirectoryInfo(pathModel);
            FileInfo file = new FileInfo(fileName);
            List<string> result = new List<string>();
            // Проверка на существование указанной директории.
            if (directory.Exists)
            {
                Console.WriteLine("Ищу файл... в");
                Console.WriteLine("Директория с именем: {0}", directory.FullName);
                // FileMode.OpenOrCreate - ЕСЛИ: существует ТО: открыть 
                // FileAccess.Read - только для чтения,
                // FileShare.None - Совместный доступ - Нет.
                FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                StreamReader streamR = new StreamReader(stream);
                string line;
                while ((line = streamR.ReadLine()) != null)
                {
                    result.Add(line);
                }
                stream.Close();
            }
            else
            {
                Console.WriteLine("Директория с именем: {0}  не существует.", directory.FullName);
                throw new Exception("Директория отсутсвует!");
            }
            return result;

        }

        private static void InitializeListFilesForSave(string fio, int branch, List<string> place)
        {
            for (int i = 1; i <= place.Count; i++)
            {
                if (i % 2 == 0)
                {
                    files.Add(new FileInfo(Path.Combine(pathModel,"IRBISB_"+ place[i - 1] + fio +".ini")));
                    files.Add(new FileInfo(Path.Combine(pathModel, "IRBISC_" + place[i - 1] + fio + ".ini")));
                }
            }
        }

        public static void WriteFiles(string fio, int branch, List<string> place, List<string> storingPlace)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if ((i+1) % 2 == 0)
                {
                    try
                    {
                        // FileMode.OpenOrCreate - ЕСЛИ: существует ТО: открыть ИНАЧЕ: создать новый
                        // FileAccess.Read - только для чтения,
                        // FileShare.None - Совместный доступ - Нет.
                        //FileStream stream = files[i].Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                        //StreamWriter streamW = new StreamWriter(stream);
                        //streamW.WriteLine("[@irbisc]");
                        //streamW.WriteLine("");
                        //streamW.WriteLine("[PRIVATE]");
                        //streamW.WriteLine("FIO=" + place[i] + fio);
                        string[] str = 
                            {
                                "[@irbisc]",
                                "",
                                "[PRIVATE]",
                                "FIO=" + place[i] + fio
                            };
                        File.WriteAllLines(files[i].FullName, str, Encoding.GetEncoding(1251));
                        //streamW.Close();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Не удалось сохранить данные в файл irbisc!");
                    }
                }
                else 
                {
                    try
                    {
                        // FileMode.OpenOrCreate - ЕСЛИ: существует ТО: открыть ИНАЧЕ: создать новый
                        // FileAccess.Read - только для чтения,
                        // FileShare.None - Совместный доступ - Нет.
                        //FileStream stream = files[i].Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                        //StreamWriter streamW = new StreamWriter(stream);
                        //streamW.WriteLine("[@irbisb]");
                        //streamW.WriteLine("");
                        //streamW.WriteLine("[Request]");
                        //streamW.WriteLine("MaskMrg=" + place[i]);
                        //streamW.WriteLine("MaskStore=" + storingPlace[branch-1]);
                        //streamW.WriteLine("");
                        //streamW.WriteLine("[MAIN]");
                        //streamW.WriteLine("OTVFACE=" + place[i+1] + fio);
                        //streamW.WriteLine("");
                        //streamW.WriteLine("[PRIVATE]");
                        //streamW.WriteLine("FIO=" + place[i+1] + fio);
                        //streamW.Close();
                        string[] str =
                            {
                                "[@irbisb]",
                                "",
                                "[Request]",
                                "MaskMrg=" + place[i],
                                "MaskStore=" + storingPlace[branch-1],
                                "",
                                "[MAIN]",
                                "OTVFACE=" + place[i+1] + fio,
                                "",
                                "[PRIVATE]",
                                "FIO=" + place[i+1] + fio
                            };
                        File.WriteAllLines(files[i].FullName, str, Encoding.GetEncoding(1251));
                    }
                    catch (Exception)
                    {
                        throw new Exception("Не удалось сохранить данные в файл irbisb!");
                    }
                }
            }

        }
    }
}
