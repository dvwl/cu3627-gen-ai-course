// Inventory System - Half-baked Demo
// Features: Add/view items, update quantity
using System;
using System.Collections.Generic;

class Item
{
    public string Name { get; set; }
    public int Quantity { get; set; }
}

class Program
{
    static List<Item> items = new List<Item>();
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("1. Add Item\n2. View Items\n3. Update Quantity\n4. Exit");
            var choice = Console.ReadLine();
            if (choice == "1") AddItem();
            else if (choice == "2") ViewItems();
            else if (choice == "3") UpdateQuantity();
            else if (choice == "4") break;
        }
    }
    static void AddItem()
    {
        // to do: add item
    }
    static void ViewItems()
    {
        // to do: view items
    }
	static void UpdateQuantity()
	{
		ViewItems();
		// to do: update quantity
		// to do: handle invalid input
		// to do: use LINQ where appropriate
    }
}
