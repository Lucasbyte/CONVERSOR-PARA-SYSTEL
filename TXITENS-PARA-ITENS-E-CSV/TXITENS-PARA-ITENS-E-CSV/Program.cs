using System;
using System.IO;
using System.Collections.Generic;
using System.Text;



namespace System.Threading;


class Program
{


    static void Main()
    {

        Console.WriteLine("Hello, World!");
        while (true)
        {
            string arquivo = "../txitens.txt";

            if (File.Exists(arquivo))
            {
                List<string> list_of_lines =  itensReader(arquivo);
                Thread.Sleep(1000);
                itensWriter(list_of_lines);
                Thread.Sleep(1000);
                List<Plu> ListPlu = itensAnalise(arquivo);
                Thread.Sleep(1000);
                SystelCSVWriter(ListPlu);
                Thread.Sleep(1000);
                Console.WriteLine("Fim");
            }
           else Thread.Sleep(1000);
        }




        static List<Plu> itensAnalise(string arquivo)
        {
            List<Plu> list = new List<Plu>();  
            using(StreamReader txitens = new StreamReader(arquivo)) 
            {
                while(!txitens.EndOfStream)
                {
                    
                    string line = txitens.ReadLine().Replace("\n", "");
                    string secao = line.Substring(0, 2);
                    string plu_cod = line.Substring(5, 6);
                    string desc = "";
                    if (line.Length <= 71)
                    {
                        desc = line.Substring(20).Replace("  ", "");
                    }
                    else desc = line.Substring(20, 15).Replace("  ", "");
                    string preco = line.Substring(11, 6);
                    int vendaNum = int.Parse(line.Substring(4, 1));
                    string validade = line.Substring(17, 3);
                    string receita = "";
                    if(line.Length > 71)
                    {
                        receita = line.Substring(75).Replace("  ", "");
                    }
                    Plu plu = new Plu(plu_cod, secao, desc, preco, vendaNum, validade, receita);
                    list.Add(plu);
                }
            }
            
            return list;
        } 

        static List<String> itensReader(string arquivo) 
        {
            using(StreamReader itens = new StreamReader(arquivo))
            {
                List<String> list_of_lines = new List<String>(); 
                while(!itens.EndOfStream)
                {
                    string line = itens.ReadLine();
                    if(line.Length < 71)
                    {
                        string vaz = new string(' ', (71 - line.Length));
                        line = line + vaz;
                    }
                    list_of_lines.Add(line);
                }
                return list_of_lines;
            }
        } 

        static void itensWriter(List<string> list_of_lines) 
        {
           using(StreamWriter txitens = new StreamWriter("Itens.txt")) 
            {
                foreach(string line in list_of_lines)
                {
                    txitens.WriteLine(line);
                }
            } 
        }

        static void SystelCSVWriter(List<Plu> plus) 
        {
            using(StreamWriter CSV = new StreamWriter("SYSTEL_CSV.csv")) 
            {
                foreach (var plu in plus)
                {
                    string venda_systel = plu.venda == 0 ? "PESO     " : "UNIDAD   ";
                    double preco = double.Parse(plu.preco) / 100;
                    string line_csv = $"SECAO {plu.secao};{plu.plu_cod};{plu.nome};{plu.plu_cod};{preco};{preco};{venda_systel};{plu.validade};;0;0;{plu.receita};;;N;0;0;0;;0;0;0;0;0";
                    CSV.WriteLine(line_csv);
                }
            }
        } 
    }
}

class Plu
{
    public string plu_cod = "";
    public string secao = "";
    public string nome = "";
    public string preco = "";
    public int venda = 0;
    public string validade = "";
    public string receita = "";

    public Plu(string plu_cod, string secao, string nome, string preco, int venda, string validade, string receita)
    {
        this.plu_cod = plu_cod;
        this.secao = secao;
        this.nome = nome;
        this.preco = preco;
        this.venda = venda;
        this.validade = validade;
        this.receita = receita;
    }
}
