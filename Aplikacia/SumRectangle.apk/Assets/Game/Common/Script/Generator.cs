
using System.Collections.Generic;
using UnityEngine;

public class Generator_uloh
{
    public List<Vrchol> pole;
    public List<int> table;
    public List<int> zasobnik;
    public List<int> visited;
    public int pom_suc0;
    public int pom_suc1;
    public int pom_suc2;
    public Vrchol x;
    public int level;

    public Generator_uloh(int level = 0)
    {
        this.level = level;
    }

    public List<int> get_array(int velkost)
    {
        pole = new List<Vrchol>() { };
        for (int i = 0; i < velkost; i++)
        {
            this.pole.Add(new Vrchol(0, i, null, null));
        }
        this.pole[0].left = this.pole[1];
        this.pole[0].right = this.pole[2];
        if (this.level > 1)
        {
            this.pole[1].left = this.pole[3];
            this.pole[1].right = this.pole[4];
            this.pole[2].left = this.pole[4];
            this.pole[2].right = this.pole[5];
        }
        if (this.level > 3)
        {
            this.pole[3].left = this.pole[6];
            this.pole[3].right = this.pole[7];
            this.pole[4].left = this.pole[7];
            this.pole[4].right = this.pole[8];
            this.pole[5].left = this.pole[8];
            this.pole[5].right = this.pole[9];
        }

        this.pom_suc0 = Random.Range(1, 3);
        this.pom_suc1 = Random.Range(3, 6);
        this.pom_suc2 = Random.Range(6, 10);

        if (this.level == 1)
        {
            bool s = true;
            while (s)
            {
                this.pole[2].data = Random.Range(1, 10);
                this.pole[1].data = Random.Range(1, 10);
                this.pole[0].data = this.pole[1].data + this.pole[2].data;
                if (this.pole[0].data <= 10)
                {
                    s = false;
                }
            }
        }

        if (this.level == 2)
        {
            bool s = true;
            while (s)
            {
                this.pole[5].data = Random.Range(1, 12);
                this.pole[4].data = Random.Range(1, 12);
                this.pole[3].data = Random.Range(1, 12);
                this.pole[2].data = this.pole[5].data + this.pole[4].data;
                this.pole[1].data = this.pole[4].data + this.pole[3].data;
                this.pole[0].data = this.pole[2].data + this.pole[1].data;
                if (this.pole[0].data <= 30)
                {
                    s = false;
                }
            }
        }

        if (this.level == 3)
        {
            this.pole[this.pom_suc0].data = Random.Range(13, 21);
            this.pole[this.pom_suc1].data = 25 - this.pole[this.pom_suc0].data;
        }
        if (this.level >= 4)
        {
            this.pole[this.pom_suc0].data = Random.Range(23, 35);
            this.pole[this.pom_suc1].data = 45 - this.pole[this.pom_suc0].data;
            this.pole[this.pom_suc2].data = Random.Range(1, 6);
        }

        if (this.level > 2)
        {
            if ((this.pom_suc0 == 2 && this.pom_suc1 == 3) || (this.pom_suc0 == 1 && this.pom_suc1 == 5))
            {
                this.pole[4].data = this.pole[this.pom_suc0].data / 3;
                this.pole[0].data = this.pole[this.pom_suc0].data + this.pole[this.pom_suc1].data + this.pole[4].data;
            }
            else
            {
                this.pole[0].data = 2 * this.pole[this.pom_suc0].data + this.pole[this.pom_suc1].data - 1;
            }
        }

        this.generuj();
        if (this.level > 3)
        {
            if (this.pole[6].data <= 0 || this.pole[7].data <= 0 || this.pole[8].data <= 0 || this.pole[9].data <= 0)
            {
                if (this.pole[3].data == 0)
                {
                    this.generuj();
                }

                this.pole[6].data = 0;
                this.pole[7].data = 0;
                this.pole[8].data = 0;
                this.pole[9].data = 0;
                x = this.pole[3];
                if (x.data > this.pole[4].data)
                    x = this.pole[4];
                if (x.data > this.pole[5].data)
                    x = this.pole[5];

                this.pole[x.index].left.data = x.data / 3;
                this.generuj();
                if (x.index == 5)
                    this.pole[6].data = this.pole[3].data - this.pole[7].data;
            }
        }
        this.table = new List<int>() { };
        for (int i = 0; i < this.pole.Count; i++)
            this.table.Add(this.pole[i].data);
        if (this.table.Contains(0))
            this.get_array(velkost);
        return this.table;
    }

    public void generuj()
    {
        zasobnik = new List<int> { 0, };
        visited = new List<int> { };


        while (zasobnik.Count != 0)
        {
            int v = zasobnik[0];
            zasobnik.RemoveAt(0);
            if (visited.Contains(v))
                continue;
            if (this.pole[v].left == null || this.pole[v].right == null)
                continue;
            if (this.pole[v].data != 0 && this.pole[v].left.data != 0 && this.pole[v].right.data == 0)
            {
                this.pole[v].right.data = this.pole[v].data - this.pole[v].left.data;
            }
            else if (this.pole[v].data != 0 && this.pole[v].right.data != 0 && this.pole[v].left.data == 0)
            {
                this.pole[v].left.data = this.pole[v].data - this.pole[v].right.data;
            }
            else
            {

                if (!visited.Contains(v))
                    zasobnik.Add(v);
            }
            visited.Add(v);
            if (!visited.Contains(this.pole[v].left.index))
                zasobnik.Add(this.pole[v].left.index);
            if (!visited.Contains(this.pole[v].right.index))
                zasobnik.Add(this.pole[v].right.index);
        }
    }

    public List<int> reverse(List<int> x)
    {
        List<int> pom = new List<int> { };
        for (int i = (x.Count - 1); i >= 0; i--)
        {
            pom.Add(x[i]);
        }
        return pom;
    }
}

public class Vrchol
{
    public int data;
    public int index;
    public Vrchol left;
    public Vrchol right;

    public Vrchol(int data, int index, Vrchol left = null, Vrchol right = null)
    {
        this.data = data;
        this.index = index;
        this.left = left;
        this.right = right;
    }
}


public class Kontrola
{
    public int velkost;

    public Kontrola(int velkost)
    {
        this.velkost = velkost;
    }

    public List<int[][]> Sprav_pole(List<int> pole, int p1 = 0, int p2 = 0)
    {
        List<int[][]> ret = new List<int[][]> { };
        int y = this.velkost;
        int pom = 0;
        int[][] pom_pole = new int[this.velkost][];
        int[][] p1p2 = new int[2][];
        p1p2[0] = new int[2];
        p1p2[1] = new int[2];
        for (int i = 0; i < this.velkost; i++)
        {
            pom_pole[i] = new int[y];
            for (int j = 0; j < y; j++)
            {
                if (p1 == pom)
                {
                    p1p2[0][0] = i;
                    p1p2[0][1] = j;
                    ret.Add(p1p2);
                }
                if (p2 == pom)
                {
                    p1p2[1][0] = i;
                    p1p2[1][1] = j;
                    ret.Add(p1p2);
                }
                pom_pole[i][j] = pole[pom];
                pom++;
            }
            y--;
        }
        ret.Add(pom_pole);
        return ret;
    }

    public bool Vyhodnot(List<int> v_pole, int p1 = 0, int p2 = 0, int result = 0)
    {
        bool ries = true;
        List<int[][]> pom_pole = this.Sprav_pole(v_pole, p1, p2);
        int[][] pole = pom_pole[2];
        int i1 = pom_pole[0][0][0];
        int j1 = pom_pole[0][0][1];
        int i2 = pom_pole[1][1][0];
        int j2 = pom_pole[1][1][1];
        for (int i = 0; i < pole.Length - 1; i++)
        {
            for (int j = 0; j < pole[i].Length - 1; j++)
            {
                if (pole[i][j] + pole[i][j + 1] != pole[i + 1][j])
                {
                    ries = false;
                }
            }
        }
        if (ries)
        {
            if (result != 0 && (p1 != 0 || p2 != 0))
            {
                if (pole[i1][j1] + pole[i2][j2] != result)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
public class ShuffleDictionary
{

    public Dictionary<string, string> Shuffle(Dictionary<string, string> input)
    {
        int vstupDoGeneratoraA = 0;
        int vstupDoGeneratoraB = 0;
        int pocetOpakovani = 1;
        switch (input.Count)
        {
            case 6:
                vstupDoGeneratoraA = 2;
                vstupDoGeneratoraB = 3;
                pocetOpakovani = 1;

                break;
            case 12:
                vstupDoGeneratoraA = 3;
                vstupDoGeneratoraB = 6;
                pocetOpakovani = 2;
                break;
            case 20:
                vstupDoGeneratoraA = 5;
                vstupDoGeneratoraB = 10;
                pocetOpakovani = 4;
                break;

        }

        for (var i = 0; i < pocetOpakovani; i++)
        {
            var randomA = Random.Range(0, vstupDoGeneratoraA);
            var randomB = Random.Range(vstupDoGeneratoraA, vstupDoGeneratoraB);
            string SaveA = input["SlotM_" + randomA];
            input["SlotM_" + randomA] = input["SlotM_" + randomB];
            input["SlotM_" + randomB] = SaveA;
        }
        return input;
    }

}