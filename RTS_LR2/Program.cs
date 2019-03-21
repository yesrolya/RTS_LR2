using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS_LR2
{
    class Schedule
    {
        int processes;
        int tasks;
        int[,] B;
        int[,] C;
        int[] time_vector;
        int[] L;

        public Schedule (int processes, int tasks, int[,] B, int[] time_vector)
        {
            this.processes = processes;
            this.tasks = tasks;
            this.B = new int[tasks, tasks];
            this.time_vector = new int[tasks];
            for (int i = 0; i < tasks; i++)
            {
                for (int j = 0; j < tasks; j++)
                    this.B[i, j] = B[i, j];
                this.time_vector[i] = time_vector[i];
            }
            L = new int[tasks];
            C = new int[tasks, tasks];
            CreateReachabilityMatrix();
            CreatePriorityList();
            CreateDiagramm();
        }
        //заполнение матрицы достижимости
        private void CreateReachabilityMatrix ()
        {
            for (int i = tasks - 1; i >= 0; i--)
                for (int j = 0; j < tasks; j++)
                    if (B[i,j] == 1)
                    {
                        C[i, j] = 1;
                        for (int k = 0; k < tasks; k++)
                            if (B[j, k] == 1)
                                C[i, k] = 1;
                    }
        }
        //создание списка задач в сооветствии с их приоритетом выполнения
        private void CreatePriorityList()
        {
            int[] pr1 = CountFirstCriterion();
            int[] pr2 = CountSecondCriterion();

            int temp1 = pr1.Max() + 10;

            for (int i = 0; i < tasks; i++)
            {
                List<string> lst = new List<string>();

                int minValue = pr1.Min();
                int minIndex;

                while ((minIndex = pr1.ToList().IndexOf(minValue)) != -1)
                {
                    lst.Add(pr2[minIndex] + "+" + minIndex);
                    pr1[minIndex] = temp1;
                }

                lst.Sort();
                lst.Reverse();
                foreach (var l in lst)
                {
                    L[i++] = int.Parse(l.Split('+')[1]);
                }
            }
        }
        //создание списка значений для критерия первого приоритета
        //Наименьшее значение критического по количеству вершин пути от начальной вершины до данной 
        private int[] CountFirstCriterion()
        {
            //индекс элемента в массиве соответствует номеру вершины
            //значение - максимальное кол-во вершин от 0 до данной
            int[] result = new int[tasks];
            for (int i = 1; i < tasks; i++)
                result[i] = CountVertexToStart(i);
            return result;
        }
        //подсчет максимального количества вершин от заданной до начальной
        private int CountVertexToStart(int vertex)
        {
            if (vertex == 0) return 1;

            int maxVal = 0;
            for (int i = 0; i < tasks; i++)
            {
                if (B[i,vertex] == 1)
                {
                    int temp = CountVertexToStart(i);
                    if (temp > maxVal)
                    {
                        maxVal = temp;
                    }
                }
            }
            return maxVal + 1;
        }

        //создание списка значений для критерия второго приоритета
        //Наибольшее значение критического по времени пути от данной вершины до конечной 
        private int[] CountSecondCriterion()
        {
            int[] result = new int[tasks];
            for (int i = 1; i < tasks; i++)
                result[i] = CountPathToEnd(i);
            return result;
        }

        //подсчет значения критического по времени пути от данной вершины до конечной
        private int CountPathToEnd(int vertex)
        {
            if (vertex == tasks - 1) return 0;

            int maxVal = 0;
            for (int i = 0; i < tasks; i++)
            {
                if (B[i, vertex] == 1)
                {
                    int temp = CountPathToEnd(i);
                    if (temp > maxVal)
                    {
                        maxVal = temp;
                    }
                }
            }
            return maxVal + time_vector[vertex];
        }

        private void CreateDiagramm()
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Вариант 8 {0}" +
                              "3 процессора, " +
                              "11 задач{0}" +
                              "Критерии:{0}" +
                              "Наименьшее значение критического по количеству вершин пути от начальной вершины до данной/{0}" +
                              "Наибольшее значение критического по времени  пути от данной вершины до конечной{0}",
                              Environment.NewLine);
            //наименования переменных соответствуют методичке
            int m = 3; //количество процессоров
            int n = 11; //количество задач
                        //вектор времен
            int[] T = { 3, 2, 2, 3, 4, 2, 3, 4, 2, 3, 4 };
            //матрица смежности
            int[,] B = { {0,1,1,1,0,0,0,0,0,0,0},
                     {0,0,0,0,0,1,0,0,0,0,0},
                     {0,0,0,0,1,0,0,0,0,0,0},
                     {0,0,0,0,0,1,0,1,0,0,0},
                     {0,0,0,0,0,0,1,0,0,0,0},
                     {0,0,0,0,0,0,0,0,1,0,0},
                     {0,0,0,0,0,0,0,1,0,0,0},
                     {0,0,0,0,0,0,0,0,1,1,0},
                     {0,0,0,0,0,0,0,0,0,0,1},
                     {0,0,0,0,0,0,0,0,0,0,1},
                     {0,0,0,0,0,0,0,0,0,0,0}};
            //матрица достижимости
            int[,] C = new int[n,n];
            //список задач в соответствии с приоритетом
            int[] L = new int[n];
            int[] priority1 = new int[n];
            int[] priority2 = new int[n];

            priority1[0] = 0;

            for(int j = 1; j < n; j++)
            {

            }
            
            Console.ReadKey();
        }
    }
}
