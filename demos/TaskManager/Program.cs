// Task Manager - Half-baked Demo
// Features: Add, view, delete tasks
using System;
using System.Collections.Generic;

class Program
{
    static List<string> tasks = new();
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("1. Add Task\n2. View Tasks\n3. Delete Task\n4. Exit");
            var choice = Console.ReadLine();
            if (choice == "1") AddTask();
            else if (choice == "2") ViewTasks();
            else if (choice == "3") DeleteTask();
            else if (choice == "4") break;
        }
    }
	static void AddTask()
	{
		// to do: add task
    }
    static void ViewTasks()
    {
        // to do: view tasks
    }
	static void DeleteTask()
	{
		ViewTasks();
		// to do: delete task
		// to do: confirm deletion
		// to do: handle invalid input
    }
}
