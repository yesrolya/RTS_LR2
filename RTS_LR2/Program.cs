using System;
using System.Collections.Generic;
using System.Linq;

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
        List<int>[] A;
        int sum_time;

        public Schedule(int processes, int tasks, int[,] B, int[] time_vector)
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
            InitSchedule();
            CreateReachabilityMatrix();
            CreatePriorityList();
            CreateDiagramm();
        }

        void InitSchedule()
        {
            A = new List<int>[processes];
            for (int i = 0; i < processes; i++)
                A[i] = new List<int>();
        }

        //заполнение матрицы достижимости
        private void CreateReachabilityMatrix()
        {
            for (int i = tasks - 1; i >= 0; i--)
                for (int j = 0; j < tasks; j++)
                    if (B[i, j] == 1)
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
                if (B[i, vertex] == 1)
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
            List<Zadacha> zadachi = new List<Zadacha>(); //задачи на выполнении
            List<int> completed = new List<int>(); //завершенные задачи
            int time = 1;
            int j = 0;
            while (j < tasks)
            {
                int tempTaskNum = -1;
                if (CouldBeLoaded(L[j], completed))
                    tempTaskNum = L[j];
                bool setIt = false;
                for (int i = 0; i < processes; i++)
                {
                    if (A[i].Count() < time)
                    {
                        SetToSchedule(i, tempTaskNum);
                        if (tempTaskNum != -1)
                        {
                            setIt = true;
                            zadachi.Add(new Zadacha(tempTaskNum, time_vector[tempTaskNum]));
                            tempTaskNum = -1;
                        }
                    }
                }

                for (int k = zadachi.Count() - 1; k >= 0; k--)
                {
                    zadachi[k] = zadachi[k] + 1;
                    if (zadachi[k].Completed())
                    {
                        completed.Add(zadachi[k].number);
                        zadachi.RemoveAt(k);
                    }
                }
                if (setIt) j++;    
                time++;

            }
        }
        
        private void SetToSchedule(int process_number, int task_number)
        {
            //ставит в вектор процесса задачу, согласно ее продолжительности
            //-1 - отсутствие задачи в расписании
            int time = task_number == -1 ? 1 : time_vector[task_number];
            for (int i = 0; i < time; i++)
                A[process_number].Add(task_number);
        }


        private bool CouldBeLoaded (int current, List<int> completed)
        {
            int counter = 0;
            for (int i = 0; i < tasks; i++)
                if (B[i, current] == 1) counter++;
            if (counter == 0) return true;
            foreach (var c in completed)
                if (B[c, current] == 1)
                    return true;
            return false;
        }
        
    }

    public class Zadacha
    {
        public int number;
        public int duration;

        public Zadacha (int number, int duration)
        {
            this.number = number;
            this.duration = duration;
        }

        public static Zadacha operator + (Zadacha z1, int num)
        {
            return new Zadacha(z1.number, z1.duration + num);
        }

        public static Zadacha operator -(Zadacha z1, int num)
        {
            return new Zadacha(z1.number, z1.duration - num);
        }

        public bool Completed ()
        {
            if (duration == 0) return true;
            else return false;
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
            int[,] B = {{0,1,1,1,0,0,0,0,0,0,0},
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
            
            
            Console.ReadKey();
        }
    }
}
