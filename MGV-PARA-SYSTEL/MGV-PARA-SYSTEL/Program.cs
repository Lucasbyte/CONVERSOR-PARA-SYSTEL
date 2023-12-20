using System;
using System.IO;
using System.Collections.Generic;
using System.Text;



namespace System.Threading;


class Program
{

    static void Main()
    {
        while (true)
        {
            string arquivo = "../itensmgv.txt";
            string receita = "../txinfo.txt";
            string nutri = "../infnutri.txt";

            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();                     
            if (File.Exists(arquivo))
            {
                Console.WriteLine("INICIO");
                itensWrite(arquivo);
                Thread.Sleep(1000);
                dict = itensAnalise("itenssystel.txt");
            }
            else if (File.Exists(arquivo.Replace(".txt", ".bak")))
            {
                Console.WriteLine("INICIO");
                itensWrite(arquivo.Replace(".txt", ".bak"));
                arquivo = arquivo.Replace(".txt", ".bak");
                Thread.Sleep(1000);
                dict = itensAnalise("itenssystel.txt");
            }
            if (File.Exists(receita) && File.Exists(arquivo))
            {
                Thread.Sleep(1000);
                receitaWriter(receita, dict);
                //Console.WriteLine("FIM");
                Thread.Sleep(1000);
            }
            else if (File.Exists(receita.Replace(".txt", ".bak")) && File.Exists(arquivo))
            {
                receita = receita.Replace(".txt", ".bak");
                Thread.Sleep(1000);
                receitaWriter(receita, dict);
                //Console.WriteLine("FIM");
                Thread.Sleep(1000);
            }
            if (File.Exists(nutri) && File.Exists(arquivo))
            {
                Thread.Sleep(1000);
                nutriWriter(nutri, dict);
                Console.WriteLine("FIM");
            }
            else if (File.Exists(nutri.Replace(".txt", ".bak")) && File.Exists(arquivo))
            {
                nutri = nutri.Replace(".txt", ".bak");
                Thread.Sleep(1000);
                nutriWriter(nutri, dict);
                Console.WriteLine("FIM");
            }

        }
        static Dictionary<string, List<string>> itensAnalise(string arquivo)
        {
            Dictionary<string, List<string>> itensDict = new Dictionary<string, List<string>>();
            using (StreamReader itensmgv = new StreamReader(arquivo))
            {
                while (!itensmgv.EndOfStream)
                {
                    string line = itensmgv.ReadLine();
                    string plu_cod = line.Substring(3, 6);
                    string receita_cod = line.Substring(68, 6);
                    string nutri_cod = line.Substring(78, 6);



                    List<string> list = new List<string>() { receita_cod, nutri_cod };
                    itensDict[plu_cod] = list;
                }
            }
            return itensDict;
        }
        static void itensWrite(string arquivo)
        {
            List<string> linelist = new List<string>();
            using (StreamReader itensmgv = new StreamReader(arquivo))
            {
                while (!itensmgv.EndOfStream)
                {
                    string plu = "";
                    string linetxt = "";
                    string conversaoCaracteres = "000000|01|                                                                      0000000000000000000000000||0||0000000000000000000000";
                    string vaz = new string(' ', 25);
                    string line = itensmgv.ReadLine();
                    if (line.Length < 60)
                    {
                        line = line.Replace("\n", "");
                        line = line + " " + itensmgv.ReadLine();
                    }
                    if (line.Length == 147)
                    {
                        plu = line.Substring(3, 6);
                        linetxt = line.Substring(0, 43) + vaz + plu + "000" + line.Substring(74).Replace("\n", "") + conversaoCaracteres;
                        linelist.Add(linetxt);
                    }
                    else
                    {
                        plu = line.Substring(3, 6);
                        linetxt = line.Substring(0, 43) + vaz + plu + line.Substring(74).Replace("\n", "") + conversaoCaracteres;
                        linelist.Add(linetxt);
                    }
                }
            }
            using (StreamWriter itensSystel = new StreamWriter("itensSystel.txt"))
            {
                foreach (string line in linelist)
                {
                    itensSystel.WriteLine(line);
                }
            }
        }

        static void receitaWriter(string arquivoReceita, Dictionary<string, List<string>> itensDict)
        {
            Dictionary<string, string> recDict = new Dictionary<string, string>();

            List<string> Receita_valid_cod_list = new List<string>();
            foreach (List<string> cod in itensDict.Values)
            {
                Receita_valid_cod_list.Add(cod[0]);
            }
            Console.WriteLine(Receita_valid_cod_list[0]);
            using (StreamReader receitamgv = new StreamReader(arquivoReceita))
            {
                while (!receitamgv.EndOfStream)
                {
                    string line = receitamgv.ReadLine();
                    line = line.Replace("\n", "");
                    string receita_cod = line.Substring(0, 6);
                    if (line.Length > 107)
                    {
                        string receita = line.Substring(0, 6) + line.Substring(106)
                            .Replace(": ", ":")
                            .Replace(" ( ", "(")
                            .Replace(" ) ", ")");

                        recDict[receita_cod] = receita;
                    }
                }

            }
            using (StreamWriter receitaSystel = new StreamWriter("receitasSystel.txt"))
            {
                foreach (string receita in recDict.Values)
                {
                    string cod = receita.Substring(0, 6);
                    if (Receita_valid_cod_list.Contains(cod))
                    {
                        receitaSystel.WriteLine(receita);
                    }
                }
            }
        }
        static void nutriWriter(string arquivoNutri, Dictionary<string, List<string>> itensDict)
        {
            Dictionary<string, string> nutriDict = new Dictionary<string, string>();

            List<string> Nutri_valid_cod_list = new List<string>();
            foreach (List<string> cod in itensDict.Values)
            {
                Nutri_valid_cod_list.Add(cod[1]);
            }
            Console.WriteLine(Nutri_valid_cod_list[0]);
            using (StreamReader nutrimgv = new StreamReader(arquivoNutri))
            {
                while (!nutrimgv.EndOfStream)
                {
                    bool notZero = true;
                    string line = nutrimgv.ReadLine().Replace("\n", "");
                    if (line.Length == 45)
                    {
                        if (line.Substring(17, 28) == "0000000000000000000000000000")
                        {
                            notZero = false;
                        }
                        line = line + "0000|000";
                        string porcao = (line.Substring(7, 4) != "0000") ? line.Substring(7, 3) : "01000";
                        line = line + porcao;
                        line = line + "0" + line.Substring(12, 14);
                        line = line + "000000" + line.Substring(26, 24);
                        line = line + "000000000";
                    }
                    else line = line.Replace("|", "0000|");

                    if (int.Parse(line.Substring(61, 2)) > 28)
                    {
                        line = line.Substring(0, 61) + "16" + line.Substring(63);
                    }
                    bool boo = line.Substring(7, 103) != "000000000000000000000000000000000000000000|000000000000000000000000000000000000000000000000000000000000";


                    string cod_nutri = line.Substring(1, 6);

                    if (boo && notZero)
                    {
                        nutriDict[cod_nutri] = line;
                    }
                }
            }
            using(StreamWriter  systelNutri = new StreamWriter("nutriSystel.txt")) 
            {
                foreach(string nutri in nutriDict.Values)
                {
                    string cod = nutri.Substring(1, 6);
                    if (Nutri_valid_cod_list.Contains(cod))
                    {
                        systelNutri.WriteLine(nutri);
                    }
                }
                    
            } 

        }
    }
}