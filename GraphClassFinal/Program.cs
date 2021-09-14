using System;

namespace Graph_Rogova
{
    class Program
    {
        static void Main()
        {
            Graph graph = null;
            Graph graph2 = null;
            Console.Write("\"1\" - файловый ввод, \"0\" - пустой граф: ");
            bool file = Convert.ToBoolean(int.Parse(Console.ReadLine()));
            Console.Write("\"1\" - граф взвешанный, \"0\" - граф невзвешанный: ");
            bool weighted = Convert.ToBoolean(int.Parse(Console.ReadLine()));
            Console.Write("\"1\" - граф ориентированный, \"0\" - граф неориентированный: ");
            bool orientation = Convert.ToBoolean(int.Parse(Console.ReadLine()));

            if (file)
            {
                graph = new Graph("input5.txt", weighted, orientation);
            }
            else
            {
                graph = new Graph();
            }

            graph.Weighted = weighted;
            graph.Orientation = orientation;

            bool x = true;
            while (x)
            {
                Console.WriteLine("" +
                    "\n1) Добавить вершину" +
                    "\n2) Добавить ребро" +
                    "\n3) Удалить вершину" +
                    "\n4) Удалить ребро" +
                    "\n5) Вывести список смежности" +
                    "\n6) Вывести список ребер" +
                    "\n7) Найти полустепень захода данной вершины орграфа" +
                    "\n8) Вывести номера вершин орграфа, в которых есть петли" +
                    "\n9) Вывести список смежности графа, являющегося объединением двух заданных" +
                    "\n10) Вывести список смежности второго графа" +
                    "\n11) Найти все вершины орграфа, недостижимые из данной" +
                    "\n12) Вывести длины кратчайших путей от всех вершин до u" +
                    "\n13) Найти каркас минимального веса во взвешенном неориентированном графе из N вершин и M ребер" +
                    "\n14) Найти центр графа — множество вершин, эксцентриситеты которых равны радиусу графа" +
                    "\n15) Вывести длины кратчайших путей от w до v1 и v2" +
                    "\n16) Определить множество вершин орграфа, расстояние от которых до заданной вершины не более N" +
                    "\n17) Выход");
                int ch = int.Parse(Console.ReadLine());
                switch (ch)
                {
                    case 1:
                        Console.Write("Номер добавляемой вершины: ");
                        int NodeNew = int.Parse(Console.ReadLine());
                        graph.AddNode(NodeNew);
                        break;

                    case 2:
                        if (graph.Weighted)
                        {
                            Console.Write("Номер вершины, из которой идёт ребро: ");
                            int NodeFirst = int.Parse(Console.ReadLine());
                            Console.Write("Номер вершины, в которую входит ребро: ");
                            int NodeSecond = int.Parse(Console.ReadLine());
                            Console.Write("Вес ребра: ");
                            int Weight = int.Parse(Console.ReadLine());
                            graph.AddRib(NodeFirst, NodeSecond, Weight);
                        }
                        else
                        {
                            Console.Write("Номер вершины, из которой идёт ребро: ");
                            int NodeFirst = int.Parse(Console.ReadLine());
                            Console.Write("Номер вершины, в которую входит ребро: ");
                            int NodeSecond = int.Parse(Console.ReadLine());
                            graph.AddRib(NodeFirst, NodeSecond);
                        }
                        break;

                    case 3:
                        Console.Write("Номер удаляемой вершины: ");
                        graph.DeleteNode(int.Parse(Console.ReadLine()));
                        break;

                    case 4:
                        Console.Write("Номера вершин, которые соединяют удаляемое ребро: ");
                        int Nodefirst = int.Parse(Console.ReadLine());
                        int Nodesecond = int.Parse(Console.ReadLine());
                        graph.DeleteRib(Nodefirst, Nodesecond);
                        break;

                    case 5:
                        graph.Show();
                        break;

                    case 6:
                        graph.ListOfRibs();
                        break;                    

                    case 7:
                        // Полустепень захода в орграфе для вершины v — число дуг, входящих в вершину
                        //1a. №3. Найти полустепень захода данной вершины орграфа.
                        Console.Write("Введите номер вершины: ");
                        int NodeZ1 = int.Parse(Console.ReadLine());
                        graph.Zadanie_1a_3(NodeZ1);
                        break;

                    case 8:
                        //1a. №11. Вывести номера вершин орграфа, в которых есть петли.
                        var k = graph.Zadanie_1a_11();
                        if (k.Count == 0)
                            Console.WriteLine("В графе нет вершин с петлями");
                        foreach (var item in graph.Zadanie_1a_11())
                        {
                            Console.WriteLine(item);
                        }
                        break;

                    case 9:
                        //1b. №3. Вывести список смежности графа, являющегося объединением двух заданных.
                        Console.WriteLine("Создание второго графа:");                        
                        Console.Write("\"1\" - файловый ввод, \"0\" - пустой граф: ");
                        bool file2 = Convert.ToBoolean(int.Parse(Console.ReadLine()));
                        Console.Write("\"1\" - граф взвешанный, \"0\" - граф невзвешанный: ");
                        bool weighted2 = Convert.ToBoolean(int.Parse(Console.ReadLine()));
                        Console.Write("\"1\" - граф ориентированный, \"0\" - граф неориентированный: ");
                        bool orientation2 = Convert.ToBoolean(int.Parse(Console.ReadLine()));
                        // оба графа неориентированные (взвешанные/невзвешанные)
                        if (file2)
                        {
                            graph2 = new Graph("input1.txt", weighted2, orientation2);
                        }
                        else
                        {
                            graph2 = new Graph();
                        }
                        graph2.Weighted = weighted2;
                        graph2.Orientation = orientation2;
                        (graph + graph2).Show();
                        break;

                    case 10:
                        graph2.Show();
                        break;

                        // ориентированный граф в файле input3
                    case 11:
                        //2. №2. Найти все вершины орграфа, недостижимые из данной.
                        Console.Write("Введите номер вершины: ");
                        int NodeZ3 = int.Parse(Console.ReadLine());
                        graph.vis();
                        graph.res();
                        Console.Write("Недостижимые вершины из {0}: ", NodeZ3);
                        foreach (var item in graph.Dfs(NodeZ3))
                        {
                            Console.Write(item + " ");
                        }
                        if (graph.Dfs(NodeZ3).Count == 0)
                        {
                            Console.Write("не найдены");
                        }
                        break;

                        // input3
                    case 12:
                        //2. №30. Вывести длины кратчайших путей от всех вершин до u.
                        Console.Write("Введите номер вершины u: ");
                        int u = int.Parse(Console.ReadLine());
                        graph.Shortest_Paths(u);
                        break;

                        // input/4 (взвеш и неориент)
                    case 13:
                        //3. Краскал. Дан взвешенный неориентированный граф из N вершин и M ребер. Требуется найти в нем каркас минимального веса
                        graph.Kruskals();
                        break;

                        // input5/6/7
                    case 14:
                        //4a. Дейкстра. Найти центр графа — множество вершин, эксцентриситеты которых равны радиусу графа                       
                        graph.vis();
                        foreach (var it in graph.Centers())
                        {
                            Console.Write(it + " ");
                        }
                        Console.WriteLine();
                        break;

                    case 15:
                        //4b. Форд-Беллман. Вывести длины кратчайших путей от w до v1 и v2.
                        Console.Write("Введите номер вершины w: ");
                        int w = int.Parse(Console.ReadLine());
                        Console.Write("Введите номер вершины v1: ");
                        int v1 = int.Parse(Console.ReadLine());
                        Console.Write("Введите номер вершины v2: ");
                        int v2 = int.Parse(Console.ReadLine());
                        if (!graph.ExistNode(w))
                        {
                            Console.WriteLine("Не существует вершины из которой нужно найти путь");
                        }
                        else if (!graph.ExistNode(v1))
                        {
                            Console.WriteLine("Не существует вершины до которой нужно найти путь");
                        }
                        else if (!graph.ExistNode(v2))
                        {
                            Console.WriteLine("Не существует вершины до которой нужно найти путь");
                        }
                        else
                        {
                            graph.PathFordBellman(w, v1, v2);
                        }                        
                        break;

                    case 16:
                        // input5
                        //4c. Флойд и Форд-Беллман. Определить множество вершин орграфа, расстояние от которых до заданной вершины не более N.
                        Console.Write("Введите номер заданной вершины: ");
                        int b = int.Parse(Console.ReadLine());
                        Console.Write("Введите число N: ");
                        int N = int.Parse(Console.ReadLine());
                        if (!graph.ExistNode(b))
                        {
                            Console.WriteLine("Не существует вершины до которой нужно найти путь");
                        }
                        else
                        {
                            var paths = graph.FordBellmanNegative(b);
                            Console.Write("Вершины от которых расстояние до {0} <= {1}: ", b, N);
                            if (paths == null)
                            {
                                Console.WriteLine("Вершина участвует в отрицательном цикле");
                            }
                            else
                            {
                                foreach (var m in paths.Keys)
                                {
                                    if (paths[m] <= N && paths[m] != 0)
                                    {
                                        Console.Write(m + " ");
                                    }
                                }
                            }                            
                        }                       
                        break;
                    

                    case 17:
                        x = false;
                        break;
                }
            }
        }
    }
}