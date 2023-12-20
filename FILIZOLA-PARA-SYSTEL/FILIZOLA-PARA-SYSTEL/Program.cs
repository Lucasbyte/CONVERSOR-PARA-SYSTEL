using System;
using System.IO;
using System.Collections.Generic;
using System.Text;



namespace System.Threading;


class Program
{

    static void Main()
    {
        // See https://aka.ms/new-console-template for more information
        Console.WriteLine("Hello, World!");
        while (true)
        {
            string arquivo = "../cadtxt.txt";
            string recextra = "../recextra.txt";
            string rec_ass = "../rec_ass.txt";

            if (File.Exists(arquivo))
            {
                Thread.Sleep(1000);
                {
                    Dictionary<string, List<string>> dict_plu = itensAnalize(arquivo);
                    Dictionary<string, List<string>> dict_nutri = Nutri(arquivo);
                    Thread.Sleep(1000);
                    itensWriter(arquivo, dict_plu);
                    
                    receitaWriter(recextra, rec_ass);
                    
                    nutriWriter(arquivo, dict_nutri);
                    Thread.Sleep(1000);
                    Console.WriteLine("Pronto");
                }
            }
        }
    }

    static Dictionary<string, List<string>> itensAnalize(string arquivo)
    {
        Dictionary<string, List<string>> itensDict = new Dictionary<string, List<string>>();
        using (StreamReader cadtxt = new StreamReader(arquivo))
        {
            while (!cadtxt.EndOfStream)
            {
                string linha = cadtxt.ReadLine();
                string cod_plu = linha.Substring(0, 6);
                string venda = linha.Substring(6, 1);
                string descricao = linha.Substring(7, 22);
                string valor = linha.Substring(30, 6);
                string validade = linha.Substring(36, 3);
                List<string> plu = new List<string> { cod_plu, venda, descricao, valor, validade };
                itensDict[cod_plu] = plu;
                //Thread.Sleep(1000);

                Console.Write($"{itensDict[cod_plu][2]}<-LINHA \n");
            }
        }
        return itensDict;
    }
    static Dictionary<string, List<string>> Nutri(string arquivo)
    {
        Dictionary<string, List<string>> nutriDict = new Dictionary<string, List<string>>();
        using (StreamReader cadtxt = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
        {
            while (!cadtxt.EndOfStream)
            {
                string linha = cadtxt.ReadLine();
                string cod_plu = linha.Substring(0, 6);
                string tabela = linha.Substring(39, 1);
                if (tabela == "@")
                {
                    string peso = linha.Substring(40, 4);
                    if (peso.Contains("g") || peso.Contains("G"))
                    {
                        peso = peso.Replace("g", "").Replace("G", "").Replace(" ", "");
                        if (peso.Length < 3)
                        {
                            string zero = new string('0', (3 - peso.Length));
                            peso = zero + peso;
                        }
                    }
                    else
                    {
                        peso = "100";
                    }

                    string tipo = linha.Substring(44, 18);
                    string valores = linha.Substring(75, 94);
                    //000790004017000057003000040000000000000000000000000000000800032
                    //001500007030000100004200056000120002000000000000000000000800032
                    //111110556022200074033300444044400807055502523066700000077803112
                    //111110556022200074033300444044400807055502523066700000077803112*****0000*****00000889000370000
                    string valEn = valores.Substring(1, 5);
                    string carb = valores.Substring(10, 3);
                    string proten = valores.Substring(19, 3);
                    string gordTo = valores.Substring(29, 3);
                    string gordSa = valores.Substring(37, 3);
                    string gordTr = valores.Substring(46, 3);
                    string fibra = valores.Substring(56, 3);
                    string sodio = valores.Substring(81, 5);
                    List<string> plu = new List<string> { cod_plu, peso, tipo, valEn, carb, proten, gordSa, gordTr, gordTo, fibra, sodio };
                    Console.Write($@"
{cod_plu} peso:{peso} tipo:{tipo} valores:{valores}
valEn = {valEn}
gorSat = {gordSa}
gordTr = {gordTr}
carb = {carb}
proten = {proten}
gordT = {gordTo}
fibra = {fibra}
sodio = {sodio}
<-LINHA");
                    nutriDict[cod_plu] = plu;
                }
                //Thread.Sleep(1000);


            }
        }
        return nutriDict;
    }

    static void receitaWriter(string arquivo, string arquivo_2)
    {
        Console.WriteLine("HMM");
        Dictionary<string, string> receitas = new Dictionary<string, string>();
        Dictionary<string, string> extras = new Dictionary<string, string>();
        HashSet<string> plus = new HashSet<string>();

        if (File.Exists(arquivo))
        { 
            using (StreamReader rec_ass = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
            {
                while (!rec_ass.EndOfStream)
                {
                    string rec = "";
                    string plu = "";
                    string line = rec_ass.ReadLine();
                    while (!line.Contains("@"))
                    {
                        string text = "            ";
                        if (line.Length > 12)
                        {
                            if (line.Substring(0, 12) == text)
                            {
                                plu = line.Substring(12, 6);
                                plus.Add(plu);
                                Console.WriteLine(plu);
                                rec = line.Substring(24).Replace("\n", " ");
                            }
                            else rec += line.Replace("\n", " ");
                        }
                        line = rec_ass.ReadLine();
                    }
                    receitas[plu] = rec;
                }

            }
        }
        if (File.Exists(arquivo_2))
        {
            using (StreamReader recextra = new StreamReader(arquivo_2, Encoding.GetEncoding("iso-8859-1")))
            {
                while (!recextra.EndOfStream)
                {
                    string rec = "";
                    string plu = "";
                    string line = recextra.ReadLine();
                    while (!line.Contains("@"))
                    {
                        if (line.Length > 12)
                        {
                            if (line.Substring(0, 12) == "            ")
                            {
                                plu = line.Substring(12, 6);
                                plus.Add(plu);
                                Console.WriteLine(plu);
                                rec = line.Substring(24).Replace("\n", " ");
                            }
                            else rec += line.Replace("\n", " ");
                        }
                        line = recextra.ReadLine();
                    }
                    extras[plu] = rec;
                }

            }
        }
        if (File.Exists(arquivo) || File.Exists(arquivo_2))
        {
            using (StreamWriter receitas_sysel = new StreamWriter("../receitasSystel.txt", false, Encoding.GetEncoding("iso-8859-1")))
            {
                foreach (string plu in plus)
                {
                    string extra = " ";
                    string receita = " ";
                    if (receitas.ContainsKey(plu))
                    {
                        receita = receitas[plu];
                    }

                    if (extras.ContainsKey(plu))
                    {
                        extra = extras[plu];
                    }

                    receitas_sysel.WriteLine(plu + receita + " " + extra);
                }
            }
        }
    }
    static void itensWriter(string arquivo, Dictionary<string, List<string>> dict_plu)
    {
        HashSet<string> plus = new HashSet<string>();
        using (StreamReader cadtxt = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
        {
            while (!cadtxt.EndOfStream)
            {
                string line = cadtxt.ReadLine();
                string cod_plu = line.Substring(0, 6);
                plus.Add(cod_plu);
            }
        }
        using (StreamWriter itens = new StreamWriter("../itensSystel.txt", false, Encoding.GetEncoding("iso-8859-1")))
        {
            foreach (string plu in plus)
            {
                //cod_plu, venda, descricao, valor, validade
                //010000001000699000PIMENTAO VERDE KG        PIMENTAO VERDE KG        
                string line = "01";
                string venda = "0";
                if (dict_plu[plu][1] == "U" || dict_plu[plu][1] == "u")
                {
                    venda = "1";   
                }
                string cod_plu = dict_plu[plu][0];
                string desc = dict_plu[plu][2];
                string valor = dict_plu[plu][3];
                string valid = dict_plu[plu][4];
                line = line + venda;
                line = line + cod_plu + valor + valid;
                if (desc.Length < 50) {
                    string esp = new string(' ', (50 - desc.Length));
                    desc = desc + esp;
                }
                line = line + desc + cod_plu + "0000"  + cod_plu + "11000000000000000000000000000000000000000000000000000000000000000000000|01|                                                                      0000000000000000000000000||0||0000000000000000000000";
                itens.WriteLine(line);
            }
            
        }
    }
    //cod_plu, peso, tipo, valEn, carb,  proten, gordSa, gordTr, gordTo, fibra, sodio
    static void nutriWriter(string arquivo, Dictionary<string, List<string>> dict_nutri)
    {
        HashSet<string> plus = new HashSet<string>();
        using (StreamReader cadtxt = new StreamReader(arquivo))
        {
            while (!cadtxt.EndOfStream)
            {
                string line = cadtxt.ReadLine();
                string cod_plu = line.Substring(0, 6);
                plus.Add(cod_plu);
            }
        }

        using (StreamWriter nutri = new StreamWriter("../nutriSytel.txt"))
        {
            foreach(string plu in plus)
            {
                if (dict_nutri.ContainsKey(plu)) { 
                string cod_plu = dict_nutri[plu][0];
                string peso = dict_nutri[plu][1];
                string tipo = dict_nutri[plu][2];
                    string dec = "0";
                    if (tipo.Contains("1/2") || tipo.Contains("1 / 2"))
                    {
                        dec = "3";
                    }
                    else if (tipo.Contains("2/3") || tipo.Contains("2 / 3"))
                    {
                        dec = "4";
                    }
                string qntd = "01";
                string valEn = dict_nutri[plu][3];
                string carb = dict_nutri[plu][4];
                string proten = dict_nutri[plu][5];
                string gordSa = dict_nutri[plu][6];
                string gordTr = dict_nutri[plu][7];
                string gordTo = dict_nutri[plu][8];
                string fibra = dict_nutri[plu][9];
                string sodio = dict_nutri[plu][10];
                //                  N000700010000101601710002217040039000000006000000 | 100010000101601710000000000217040039000000006000000000000000
                string line = "N" + plu + "000000000000000000000000000000000000000000|0" + "001" + peso + "0" + qntd +dec+"16" + 
                    valEn + carb +"000000"+ proten + gordTo + gordSa + gordTr + fibra + sodio + "00000000000";
                nutri.WriteLine(line);
                }
            }
        }
    }
}