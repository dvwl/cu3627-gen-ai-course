// Write a C# method to reverse a string
static string ReverseString(string input)
{
	if (input == null) return null;
	char[] chars = input.ToCharArray();
	Array.Reverse(chars);
	return new string(chars);
}

string original = "Hello, World!";
string reversed = ReverseString(original);
Console.WriteLine($"Original: {original}");
Console.WriteLine($"Reversed: {reversed}");
