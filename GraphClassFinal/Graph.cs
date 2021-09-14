using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Graph_Rogova
{
    class Graph
    {
        // 1. Структура для хранения списка смежности графа
        // словарь <ключ - вершина, значение - словарь <ключ - вершина, значение - вес>>
        // невзвешанный граф => вес равен 0
        private Dictionary<int, Dictionary<int, int>> graph;
        public bool Orientation { get; set; }
        public bool Weighted { get; set; }
        public bool[] visit; // массив непосещенных вершин
        public List<int> mas; // массив всех вершин
        public List<Edge> edg = new List<Edge>(); // список ребер
        public static int infinity = 1000000000;

        // структура для создания ребра с весом
        public struct Edge
        {
            public int v1, v2;
            public int weight;
            public Edge(int v1, int v2, int weight)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.weight = weight;
            }
        }

        // сравнение двух ребер
        int Compare(Edge e1, Edge e2)
        {
            if (e1.weight < e2.weight)
            {
                return -1;
            }
            if (e1.weight > e2.weight)
            {
                return 1;
            }
            return 0;
        }        

        // 2. Конструкторы
        // конструктор по умолчанию, создающий пустой граф
        public Graph()
        {
            graph = new Dictionary<int, Dictionary<int, int>>();
        }

        // конструктор, заполняющий данные графа из файла
        public Graph(string name, bool weighted, bool orientation)
        {
            graph = new Dictionary<int, Dictionary<int, int>>();
            string[] tmp;
            using (StreamReader file = new StreamReader(name))
            {
                // преобразование в массив строк содержимого всего файла 
                tmp = file.ReadToEnd().Split('\n').Select(x => x[x.Length - 1] == '\r' ? x.Remove(x.Length - 1) : x).ToArray<string>();
            }

            if (orientation)
            {
                if (weighted)
                {
                    // если ориентированный и взвешанный
                    foreach (var item in tmp)
                    {
                        graph.Add(int.Parse(item[0].ToString()), new Dictionary<int, int>());
                        string[] tmp0 = item.Split(' ');
                        for (int i = 0; i < tmp0.Length - 2; i += 3)
                        {
                            graph[int.Parse(item[0].ToString())].Add(int.Parse(tmp0[i + 1].ToString()), int.Parse(tmp0[i + 3].ToString()));
                        }
                    }
                }
                else
                {
                    // если ориентированный и невзвешанный
                    foreach (var item in tmp)
                    {
                        graph.Add(int.Parse(item[0].ToString()), new Dictionary<int, int>());
                        string[] tmp0 = item.Split(' ');
                        for (int i = 1; i < tmp0.Length; i++)
                        {
                            graph[int.Parse(item[0].ToString())].Add(int.Parse(tmp0[i].ToString()), 0);
                        }
                    }
                }
            }
            else
            {
                if (weighted)
                {
                    // если неориентированный и взвешанный
                    foreach (var item in tmp)
                    {
                        graph.Add(int.Parse(item[0].ToString()), new Dictionary<int, int>());
                        string[] tmp0 = item.Split(' ');
                        for (int i = 0; i < tmp0.Length - 2; i += 3)
                        {
                            graph[int.Parse(item[0].ToString())].Add(int.Parse(tmp0[i + 1].ToString()), int.Parse(tmp0[i + 3].ToString()));
                        }
                    }
                }
                else
                {
                    // если неориентированный и невзвешанный 
                    foreach (var item in tmp)
                    {
                        graph.Add(int.Parse(item[0].ToString()), new Dictionary<int, int>());
                        string[] tmp0 = item.Split(' ');
                        for (int i = 1; i < tmp0.Length; i++)
                        {
                            graph[int.Parse(item[0].ToString())].Add(int.Parse(tmp0[i].ToString()), 0);
                        }
                    }
                }
            }
        }

        // конструктор-копия
        public Graph(Graph graph1)
        {
            graph = new Dictionary<int, Dictionary<int, int>>();
            foreach (var key in graph1.graph.Keys)
            {
                graph.Add(key, new Dictionary<int, int>());
                foreach (var item in graph1.graph[key].Keys)
                {
                    graph[key].Add(item, graph1.graph[key][item]);
                }
            }
        }

        // 3. Методы:   

        // проверка на существование вершины
        public bool ExistNode(int Node)
        {
            return graph.ContainsKey(Node);
        }

        // проверка на существование ребра и вывод его веса (если нет ребра между вершинами то вывести 0)
        public int ExistReb(int Node1, int Node2)
        {
            if (graph[Node1].ContainsKey(Node2))
            {
                return graph[Node1][Node2];
            }     
            else
            {
                return infinity;
            }
        }

        // при удалении вершины меняется нумерация 
        // для заданий 4а-4с нумерация вершин идет с 0
        // метод изменяет все ключи в словаре, которые идут не последовательно, на последовательные
        // было: 0 1 2 5 6, стало 0 1 2 3 4 
        public void ChangeNodes()
        {
            int i = 0;
                foreach (var key in graph.Keys.ToArray())
                {
                    if (key != i)
                    {
                        var zn = graph[key];
                        graph.Remove(key);
                        graph.Add(i, zn);
                        foreach (var k in graph.Keys.ToArray())
                        {
                            foreach (var item in graph[k].Keys.ToArray())
                            {
                                if (item == key)
                                {
                                    var zn2 = graph[k][item];
                                    graph[k].Remove(item);
                                    graph[k].Add(i, zn2);
                                }
                            }
                        }                       
                    }
                i++;
                }                
                      
        }

        // добавить вершину
        public void AddNode(int Node)
        {
            try
            {
                graph.Add(Node, new Dictionary<int, int>());
            }
            catch
            {
                Console.WriteLine("Вершина {0} не была добавлена", Node);
            }
        }

        // добавить ребро
        public void AddRib(int Node1, int Node2)
        {
            if (ExistNode(Node1) && ExistNode(Node2))
            {
                if (Orientation)
                {
                    if (!graph[Node1].ContainsKey(Node2))
                    {
                        graph[Node1].Add(Node2, 0);
                    }
                }
                else
                {
                    if (!graph[Node1].ContainsKey(Node2))
                    {
                        graph[Node1].Add(Node2, 0);
                        if (!graph[Node2].ContainsKey(Node1))
                        {
                            graph[Node2].Add(Node1, 0);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Ребро между вершинами {0} и {1} не было добавлено", Node2, Node2);
            }
        }

        // добавить ребро во взвешенном
        public void AddRib(int Node1, int Node2, int Weight)
        {
            if (ExistNode(Node1) && ExistNode(Node2))
            {
                if (Orientation)
                {
                    if (!graph[Node1].ContainsKey(Node2))
                    {
                        graph[Node1].Add(Node2, Weight);
                    }
                    else
                    {
                        graph[Node1][Node2] = Math.Min(Weight, graph[Node1][Node2]);
                    }
                }
                else
                {
                    if (!graph[Node1].ContainsKey(Node2))
                    {
                        graph[Node1].Add(Node2, Weight);
                        if (!graph[Node2].ContainsKey(Node1))
                        {
                            graph[Node2].Add(Node1, Weight);
                        }
                    }
                    else
                    {
                        graph[Node1][Node2] = Math.Min(Weight, graph[Node1][Node2]);
                        graph[Node2][Node1] = Math.Min(Weight, graph[Node2][Node1]);
                    }

                }
            }
            else
            {
                Console.WriteLine("Ребро между вершинами {0} и {1} не было добавлено", Node1, Node2);
            }
        }


        // удалить вершину
        public void DeleteNode(int Number)
        {
            try
            {
                List<int> tmp = graph.Keys.ToList();

                if (ExistNode(Number))
                {
                    for (int i = 0; i < tmp.Count; i++)
                    {
                        for (int j = 0; j < graph[tmp[i]].Keys.Count;)
                        {
                            if (graph[tmp[i]].Keys.ToArray<int>()[j] == Number)
                            {
                                graph[tmp[i]].Remove(graph[tmp[i]].Keys.ToArray<int>()[j]);
                            }
                            j++;
                        }
                    }
                    graph.Remove(Number);
                }
                else
                {
                    Console.WriteLine("Вершина {0} не существует", Number);
                }
            }
            catch
            {
                Console.WriteLine("Вершина {0} не была удалена", Number);
            }
        }

        // удалить ребро
        public void DeleteRib(int Node1, int Node2)
        {
            if (ExistNode(Node1) && ExistNode(Node2))
            {
                if (Orientation)
                {
                    graph[Node1].Remove(Node2);
                }
                else
                {
                    graph[Node1].Remove(Node2);
                    graph[Node2].Remove(Node1);
                }
            }
            else
            {
                Console.WriteLine("Ребро между вершинами {0} и {1} не найдено", Node1, Node2);
            }
        }

        // вывод на экран
        public void Show()
        {
            if (Weighted)
            {
                foreach (var key in graph.Keys)
                {
                    Console.Write(key + " ");
                    foreach (var item in graph[key].Keys)
                    {
                        Console.Write(item + " : " + graph[key][item] + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            else
            {
                // если невзвешанный, то нули ("веса") не выводятся
                foreach (var key in graph.Keys)
                {
                    Console.Write(key + " ");
                    foreach (var item in graph[key].Keys)
                    {
                        Console.Write(item + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }

        // метод, преобразующий список смежности в список ребер (для вывода на экран)
        public void ListOfRibs()
        {
            if (Weighted)
            {
                foreach (var key in graph.Keys)
                {
                    foreach (var item in graph[key].Keys)
                    {
                        Console.WriteLine("{0}{1}, вес: {2}", key, item, graph[key][item]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            else
            {
                foreach (var key in graph.Keys)
                {
                    foreach (var item in graph[key].Keys)
                    {
                        Console.WriteLine("{0}{1}", key, item);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }

        // метод, преобразующий список смежности в список ребер
        void ToEdge()
        {
            edg.Clear();
            foreach (var key in graph.Keys)
            {
                foreach (var item in graph[key].Keys)
                {
                    edg.Add(new Edge(key, item, graph[key][item]));
                }
            }
        }

        // Задание 1а (№3). Полустепень захода в орграфе для вершины v — число дуг, входящих в вершину.
        public void Zadanie_1a_3(int NodeZ1)
        {
            if (ExistNode(NodeZ1))
            {
                int a = 0;
                foreach (var key in graph.Keys)
                {
                    foreach (var item in graph[key].Keys)
                    {
                        if (item == NodeZ1)
                            a++;
                    }
                }

                if (a != 0)
                    Console.WriteLine(a);
                else
                    Console.WriteLine("Нет входящих дуг");
            }
            else
            {
                Console.WriteLine("Вершина {0} не существует", NodeZ1);
            }
        }

        // Задание 1а (№11). Вывести номера вершин орграфа, в которых есть петли
        public List<int> Zadanie_1a_11()
        {
            List<int> loop = new List<int>();
            foreach (var key in graph.Keys)
            {
                foreach (var item in graph[key].Keys)
                {
                    if (item == key)
                        loop.Add(item);
                }
            }
            return loop;
        }

        // Задание 1b (№3). Вывести список смежности графа, являющегося объединением двух заданных (неориентированные)
        public static Graph operator +(Graph g1, Graph g2)
        {
            Graph result = new Graph();
            result.Weighted = g1.Weighted;
            result.Orientation = g1.Orientation;

            var a = g1.graph.Keys;
            var b = g2.graph.Keys;
            var x = a.Union(b);

            foreach (var t in x)
            {
                bool nod1 = g1.ExistNode(t);
                bool nod2 = g2.ExistNode(t);

                if (!result.ExistNode(t))
                {
                    result.AddNode(t);
                }

                if (nod1)
                {
                    foreach (var n1 in g1.graph[t].Keys)
                    {
                        if (g1.Weighted)
                        {
                            if (!result.ExistNode(n1))
                            {
                                result.AddNode(n1);
                                result.AddRib(t, n1, g1.graph[t][n1]);
                            }
                            if (n1 == t || !result.graph[t].ContainsKey(n1))
                            {
                                result.AddRib(t, n1, g1.graph[t][n1]);
                            }
                        }
                        else
                        {
                            if (!result.ExistNode(n1))
                            {
                                result.AddNode(n1);
                                result.AddRib(t, n1);
                            }
                            if (n1 == t || !result.graph[t].ContainsKey(n1))
                            {
                                result.AddRib(t, n1);
                            }
                        }
                    }
                }

                if (nod2)
                {
                    foreach (var n2 in g2.graph[t].Keys)
                    {
                        if (g2.Weighted)
                        {
                            if (!result.ExistNode(n2))
                            {
                                result.AddNode(n2);
                                result.AddRib(t, n2, g2.graph[t][n2]);
                            }
                            if (n2 == t && !result.graph[t].ContainsKey(n2))
                            {
                                result.AddRib(t, n2, g2.graph[t][n2]);
                            }
                        }
                        else
                        {
                            if (!result.ExistNode(n2))
                            {
                                result.AddNode(n2);
                                result.AddRib(t, n2);
                            }
                            if (n2 == t && !result.graph[t].ContainsKey(n2))
                            {
                                result.AddRib(t, n2);
                            }
                        }
                    }
                }
            }
            return result;
        }

        // пометить все вершины как непройденные
        public void vis()
        {
            visit = new bool[graph.Count];
            for (int i = 0; i < graph.Count; i++)
            {
                visit[i] = true;
            }
        }

        // получить все вершины в виде списка
        public void res()
        {
            mas = graph.Keys.ToList();
        }

        // Задание 2 (№2). Найти все вершины орграфа, недостижимые из данной.
        public List<int> Dfs(int Node3) // обход в глубину
        {
            visit[Node3 - 1] = false;
            foreach (var u in graph[Node3].Keys)
            {
                if (Node3 == u && graph[Node3].ContainsKey(u))
                {
                    mas.Remove(u);
                }
                //если вершины Node3 и u смежные, к тому же вершина u не просмотрена,
                if (graph[Node3].ContainsKey(u) && visit[u - 1])
                {
                    mas.Remove(u);
                    Dfs(u); // то рекурсивно просматриваем вершину
                }
            }
            return mas;
        }

        // Задание 2 (№30). Вывести длины кратчайших путей от всех вершин до u.
        public List<int> Bfs(int s, int f) // обход в ширину
        {
            List<int> path = new List<int>();
            int p = f;
            path.Add(p);

            Queue<int> q = new Queue<int>(); //  очередь, хранящая номера вершин           
            q.Enqueue(s);
            visit[s - 1] = false;
            Dictionary<int, int> mass = new Dictionary<int, int>();
            mass[s] = -1;            
            while (q.Count != 0)
            {
                s = q.Dequeue();           
                foreach (int i in graph[s].Keys)
                {
                    if (visit[i - 1])
                    {
                        visit[i - 1] = false;
                        q.Enqueue(i);                        
                        mass[i] = s;
                    }
                }               
            }             
            while (mass.ContainsKey(p) && mass[p] != -1)
            {
                p = mass[p];
                path.Add(p);
            }            
            return path;
        }

        public void Shortest_Paths(int u) // нахождение всех кратчайших путей до u
        {                   
            foreach (var key in graph.Keys)
            {
                vis();
                List<int> p = Bfs(key, u);
                if (p.Count == 1)
                {
                    if (graph[key].ContainsKey(u) && key == u)
                    {
                        Console.WriteLine("Существует петля в вершину u = {0}", u);
                    }
                    else
                    {
                        Console.WriteLine("Пути между вершинами {0} и {1} не существует", u, key);
                    }                    
                }
                else
                {
                    Console.Write("Кратчайший путь в вершину u = {0} из вершины {1}: ", u, key);
                    p.Reverse();
                    int count = 0;
                    foreach (var item in p)
                    {
                        count++;
                        Console.Write(item + " ");                        
                    }
                    Console.WriteLine("(длина пути = {0})", count);
                }
            }
        }       

        // Задание 3. Найти каркас минимального веса во взвешенном неориентированном графе из N вершин и M ребер
        public void Kruskals()
        {
            ToEdge();
            edg.Sort(Compare); // отсортированный список ребер 
            Dictionary<int, int> nodesSubtree = new Dictionary<int, int>();
            List<Edge> MST = new List<Edge>();
            int color = 0;
            int w = 0;
            foreach(var i in graph.Keys)
            {
                nodesSubtree.Add(i, color);
                color++;
            }
            foreach(var item in edg)
            {
                if (nodesSubtree[item.v1] != nodesSubtree[item.v2])
                {                    
                    int old_ = nodesSubtree[item.v2];
                    int new_ = nodesSubtree[item.v1];
                    MST.Add(item);
                    foreach (var key in graph.Keys)
                    {
                        if(nodesSubtree[key] == old_)
                        {
                            nodesSubtree[key] = new_;
                        }
                    }
                }
            }
            foreach(var i in MST) // вывод ребер участвующих в каркасе
            {
                Console.Write("|" + i.v1 + " " + i.v2 + "|  ");
                w += i.weight;
            }
            int x = nodesSubtree[1];
            bool g = true;
            foreach (int i in nodesSubtree.Keys) // проверка на принадлежность к одной и той же компоненте связности
            {
                if (nodesSubtree[i] != x)
                {
                    g = false;
                }
                
            }
            if (g) // если в одной компоненте, то вывести вес каркаса
            {
                Console.WriteLine("\nВес каркаса: " + w);
            }
            else
            {
                Console.WriteLine("\nГраф является лесом, а не деревом");
            }
        }

        // Эксцентриситет вершины — максимальное расстояние из всех минимальных расстояний от других вершин до данной вершины. 
        // Радиус графа — минимальный из эксцентриситетов его вершин. 
        // Задание 4a. Дейкстра. Найти центр графа — множество вершин, эксцентриситеты которых равны радиусу графа
        public int[] Dijkstra(int Node) // находит для одной вершины все ее кратчайшие пути и затем выбирает наибольший из них - эксцентриситет 
        {
            //int eccentricity = 0;            
            //List<int> nodes = mas;// Количество вершин в графе
            int[] x = new int[graph.Count]; // x[i]=0 - еще не найден кратчайший путь в i-ю вершину, x[i]=1 - кратчайший путь в i-ю вершину уже найден
            int[] t = new int[graph.Count]; //t[i] - длина кратчайшего пути от вершины Node в каждую вершину графа          
            int ew; // вес
            for (int u = 0; u < graph.Count; u++)
            {
                t[u] = infinity; //Сначала все кратчайшие пути из Node в каждую равны бесконечности
                x[u] = 0;        //и нет кратчайшего пути ни для одной вершины (пока не найдены)
            }            
            t[Node] = 0;  // Кратчайший путь из Node в Node равен 0
            x[Node] = 1;  // Отмечаем, что для вершины Node найден кратчайший путь
            int v = Node; // Делаем Node текущей вершиной

            while (true)
            {
                // Перебираем все вершины, смежные v, и ищем для них кратчайший путь
                for (int u = 0; u < graph.Count; u++)
                {
                    if (!graph[v].ContainsKey(u) || v == u) // если вершины не соединены ребром или же это одна и та же вершина
                    {
                        continue;
                    }
                    else
                    {
                        ew = graph[v][u]; // вес ребра из вершины v в u (т.е. из заданной Node в данную u)
                    }
                    if (x[u] == 0 && t[u] > t[v] + ew) // Если для вершины u еще не найден кратчайший путь и новый путь в u короче чем старый, то
                    {
                        t[u] = t[v] + ew;  // запоминаем более короткую длину пути в массив кратчайших путей t
                    }
                }
                // Ищем из всех длин некратчайших путей самый короткий
                int w = infinity;  // Для поиска самого короткого пути
                v = -1;            // В конце поиска v - вершина, в которую будет найден новый кратчайший путь. Она станет текущей вершиной
                for (int u = 0; u < graph.Count; u++) // Перебираем все вершины.
                {
                    if (x[u] == 0 && t[u] < w) // Если для вершины не найден кратчайший путь и если длина пути в вершину u меньше уже найденной, то
                    {
                        v = u; // текущей вершиной становится u-я вершина
                        w = t[u];
                    }
                }
                if (v == -1)
                {                    
                    return t;
                }
                x[v] = 1;                
            }            
        }

        // находит радиус графа — минимальный из эксцентриситетов вершин, а затем сравнивает его со всеми 
        // эксцентриситетами графа и возвращает список таких вершин
        public List<int> Centers()
        {
            ChangeNodes();
            List<int> centers = new List<int>();
            List<int> ecc = new List<int>();
            Dictionary<int, List<int>> paths = new Dictionary<int, List<int>>();
            for (int a = 0; a < graph.Count; a++)
            {
                paths.Add(a, new List<int>());
            }
            int rad = infinity;
            foreach (var key in graph.Keys) // проход по всем вершинам графа
            {
                var x = Dijkstra(key);
                for (int i = 0; i < x.Length; i++)
                {
                    paths[i].Add(x[i]);
                }
            }
            Console.WriteLine();
            Console.Write("Длина путей из ");
            for (int k = 0; k < graph.Count; k++)
            {
                Console.Write(k + " ");
            }
            Console.WriteLine("вершин соответственно (i = infinity = 1000000000)");
            for (int i = 0; i < graph.Count; i++)
            {
                Console.Write("\nк вершине " + i + ":   ");

                foreach (var y in paths[i])
                {
                    if(y == infinity)
                    {
                        Console.Write("i" + " ");
                    }
                    else
                    {
                        Console.Write(y + " ");
                    }                    
                }                
            }

            Console.Write("\nЭксцентриситеты вершин: ");
            foreach (var it in paths.Values)
            {
                ecc.Add(it.Max());
                Console.Write(it.Max() + " ");
            }
            
            foreach (var i in ecc)
            {
                if (i < rad)
                {
                    rad = i; // радиус
                }                
            }
            Console.Write("\nРадиус графа: ");
            Console.Write(rad);

            Console.Write("\nЦентр графа: ");
            for (int i = 0; i < ecc.Count; i++)
            {
                if (ecc[i] == rad)
                {
                    centers.Add(i);
                }
            }            
            return centers;
        }

        //Задание 4b. Форд-Беллман. Вывести длины кратчайших путей от u до v1 и v2.
        public List<int> FordBellman(int Node)
        {
            ToEdge(); // преобразование к списку ребер
            List<int> d = new List<int>(); // для хранения кратчайшего пути
            foreach (var i in graph.Keys) // заполнение списка бесконечностями
            { // список d имеет размерность равную количеству вершин
                d.Add(infinity);
            }
            d[Node] = 0; // для данной вершины кратчайший путь
            for (; ; )
            {
                bool any = false;
                for (int j = 0; j < edg.Count; j++) // проход по списку ребер
                {
                    // релаксация (ослабление) вдоль каждого ребра
                    if (d[edg[j].v2] > d[edg[j].v1] + edg[j].weight)
                    {
                        d[edg[j].v2] = d[edg[j].v1] + edg[j].weight;                        
                        any = true;
                    }
                }
                if (!any)
                    break;
            }
            return d;
        }
        // функция для вывода кратчайших путей от Node до v1 и v2
        public void PathFordBellman(int Node, int v1, int v2)
        {
            ChangeNodes();
            var p = FordBellman(Node); // вызов алгоритма Форда-Беллмана для данной вершины
            if (p[v1] == infinity) // если в v1 путь равен бесконечности
            { // то пути нет
                Console.WriteLine("Нет пути из " + Node + " в " + v1);
            }
            else // иначе выводятся сами длины путей из Node в v1 и v2
            {
                for (int i = 0; i < p.Count; i++)
                {
                    if (i == v1)
                    {
                        Console.WriteLine("Длина кратчайшего пути из " + Node + " в " + v1 + " = " + p[i]);
                    }
                }
            }
            // аналогично для второй вершины v2 
            // либо пути нет, если бесконечность, либо выводится кратчайший
            if (p[v2] == infinity)
            {
                Console.WriteLine("Нет пути из " + Node + " в " + v2);
            }
            else
            {
                for (int i = 0; i < p.Count; i++)
                {
                    if (i == v2)
                    {
                        Console.WriteLine("Длина кратчайшего пути из " + Node + " в " + v2 + " = " + p[i]);
                    }
                }
            }
        }



        //Задание 4c. Форд-Беллман и Флойд. Определить множество вершин орграфа, расстояние от которых до заданной вершины не более N.

        public int Floid(int start, int finish)
        {
            int size = graph.Count; // размерность графа (количество вершин)
            int[,] dp = new int[size, size]; // массив для хранения весов
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == j) // если петля
                    {
                        dp[i, j] = 0;
                    }
                    else // иначе заполняется весом текущего ребра
                    {
                        dp[i, j] = ExistReb(i, j);
                    }

                }
            }
            for (int k = 0; k < size; k++) //текущая вершина, используемая для улучшения
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        // релаксация 
                        if (dp[i, k] < infinity && dp[k, j] < infinity && i!=j && j!=k)
                        {
                            //  на k-ой фазе требуется пересчитать длины кратчайших путей между всеми парами вершин i и j следующим образом:
                            dp[i, j] = Math.Min(dp[i, j], dp[i, k] + dp[k, j]);
                        }
                        // В результате после выполнения n-ой фазы в матрице расстояний d[i][j] будет записана длина кратчайшего пути между i и j
                    }
                }
            }
            // возвращается длина кратчайшего пути между текущей вершиной и заданной
            int p = dp[start, finish];
            return p;
        }
        // для создания кратчайшего пути
        public Dictionary<int, int> PathsFloid(int finish)
        {
            ChangeNodes();
            // в paths хранится кратчайший путь 
            Dictionary<int, int> paths = new Dictionary<int, int>();
            foreach (var i in graph.Keys) // проход по всем вершинам
            {
                // вызов алгоритма Флойда для текущей вершины и заданной
                var k = Floid(i, finish);
                // добавление в paths текущей вершины с длиной кратчайшего пути
                paths.Add(i, k);
            }
            return paths;
        }
        // алгоритм Форда используется для того, чтобы исключить из 
        // рассмотрения циклы отрицательного веса
        public Dictionary<int, int> FordBellmanNegative(int Node)
        {
            ToEdge(); // преобразование к списку ребер
            List<int> d = new List<int>(); // для кратчайшего пути
            foreach (var i in graph.Keys) // заполнение бесконечностями
            {
                d.Add(infinity);
            }
            List<int> p = new List<int>(); // для отрицательного цикла
            d[0] = 0;
            foreach (var i in graph.Keys)
            { // заполнение списка для отрицательного цикла значением -1
                p.Add(-1);
            }
            int x = 0;
            for (int i = 0; i < graph.Count; ++i)
            {
                x = -1;
                for (int j = 0; j < edg.Count; ++j)
                    // критерий наличия достижимого цикла отрицательного веса: если после n-1 фазы мы выполним ещё одну фазу, 
                    // и на ней произойдёт хотя бы одна релаксация, то граф содержит цикл отрицательного веса, достижимый из v; 
                    // в противном случае, такого цикла нет.
                    if (d[edg[j].v2] > d[edg[j].v1] + edg[j].weight)
                    {
                        d[edg[j].v2] = Math.Max(-infinity, d[edg[j].v1] + edg[j].weight);
                        p[edg[j].v2] = edg[j].v1;
                        x = edg[j].v2;
                    }
            }
            int y = x;
            y = p[y];
            List<int> path = new List<int>();
            for (int cur = y; ; cur = p[cur])
            {
                path.Add(cur); // путь являющийся отрицательным циклом
                if (cur == y && path.Count > 1) break;
            }
            foreach (var k in path)
            {
                // если вершина из отрицательного цикла совпала с заданной
                // тогда вернуть значение null, т.к. необходимо исключить отрицательные циклы
                if (k == Node)
                {                   
                    return null;
                }                
            }
            return PathsFloid(Node); // вызов алгоритма Флойд для заданной вершины
        }      
    }
}