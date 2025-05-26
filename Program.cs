using System;
using System.Collections.Generic;
using System.IO;

class Note
{
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
}

class NotesApp
{
    private List<Note> notes = new List<Note>();
    private string dataFile = "notes.dat";

    static void Main(string[] args)
    {
        NotesApp app = new NotesApp();
        app.LoadNotes();
        app.ShowMenu();
    }

    private void ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== ПРОГРАММА ДЛЯ ЗАМЕТОК ===");
            Console.WriteLine("1. Просмотреть все ваши заметки");
            Console.WriteLine("2. Добавить новую заметку");
            Console.WriteLine("3. Редактировать заметку");
            Console.WriteLine("4. Удалить заметку");
            Console.WriteLine("5. Поиск заметок");
            Console.WriteLine("6. Выход");
            Console.Write("Выберите действие: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowAllNotes();
                    break;
                case "2":
                    AddNote();
                    break;
                case "3":
                    EditNote();
                    break;
                case "4":
                    DeleteNote();
                    break;
                case "5":
                    SearchNotes();
                    break;
                case "6":
                    SaveNotes();
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void ShowAllNotes()
    {
        Console.Clear();
        Console.WriteLine("=== ВСЕ ЗАМЕТКИ ===");

        if (notes.Count == 0)
        {
            Console.WriteLine("Заметок нет.");
        }
        else
        {
            for (int i = 0; i < notes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {notes[i].Title} ({notes[i].Created:dd.MM.yyyy HH:mm})");
                if (notes[i].Modified.HasValue)
                {
                    Console.WriteLine($"   Изменено: {notes[i].Modified.Value:dd.MM.yyyy HH:mm}");
                }
            }
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    private void AddNote()
    {
        Console.Clear();
        Console.WriteLine("=== ДОБАВЛЕНИЕ НОВОЙ ЗАМЕТКИ ===");

        Note note = new Note();
        Console.Write("Введите заголовок: ");
        note.Title = Console.ReadLine();

        Console.WriteLine("Введите содержимое (для завершения ввода введите пустую строку):");
        note.Content = ReadMultiLineInput();

        note.Created = DateTime.Now;
        notes.Add(note);

        Console.WriteLine("\nЗаметка добавлена. Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    private void EditNote()
    {
        Console.Clear();
        Console.WriteLine("=== РЕДАКТИРОВАНИЕ ЗАМЕТКИ ===");

        if (notes.Count == 0)
        {
            Console.WriteLine("Нет заметок для редактирования.");
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
            return;
        }

        ShowAllNotes();
        Console.Write("\nВведите номер вашей заметки для редактирования: ");

        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= notes.Count)
        {
            Note note = notes[index - 1];
            Console.Write($"Текущий заголовок ({note.Title}): ");
            string newTitle = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTitle))
            {
                note.Title = newTitle;
            }

            Console.WriteLine("Текущее содержимое:");
            Console.WriteLine(note.Content);
            Console.WriteLine("Введите новое содержимое (для завершения ввода введите пустую строку, для сохранения старого просто нажмите Enter):");
            string newContent = ReadMultiLineInput();
            if (!string.IsNullOrWhiteSpace(newContent))
            {
                note.Content = newContent;
            }

            note.Modified = DateTime.Now;
            Console.WriteLine("\nЗаметка изменена. Нажмите любую клавишу для продолжения...");
        }
        else
        {
            Console.WriteLine("\nНеверный номер заметки. Нажмите любую клавишу для продолжения...");
        }

        Console.ReadKey();
    }

    private void DeleteNote()
    {
        Console.Clear();
        Console.WriteLine("=== УДАЛЕНИЕ ЗАМЕТКИ ===");

        if (notes.Count == 0)
        {
            Console.WriteLine("Нет заметок для удаления.");
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
            return;
        }

        ShowAllNotes();
        Console.Write("\nВведите номер заметки для удаления: ");

        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= notes.Count)
        {
            notes.RemoveAt(index - 1);
            Console.WriteLine("\nЗаметка удалена. Нажмите любую клавишу для продолжения...");
        }
        else
        {
            Console.WriteLine("\nНеверный номер заметки. Нажмите любую клавишу для продолжения...");
        }

        Console.ReadKey();
    }

    private void SearchNotes()
    {
        Console.Clear();
        Console.WriteLine("=== ПОИСК ЗАМЕТОК ===");
        Console.Write("Введите текст для поиска: ");
        string searchText = Console.ReadLine().ToLower();

        Console.WriteLine("\nРезультаты поиска:");
        bool found = false;

        for (int i = 0; i < notes.Count; i++)
        {
            if (notes[i].Title.ToLower().Contains(searchText) ||
                notes[i].Content.ToLower().Contains(searchText))
            {
                Console.WriteLine($"{i + 1}. {notes[i].Title}");
                Console.WriteLine($"   {notes[i].Content.Substring(0, Math.Min(50, notes[i].Content.Length))}...");
                found = true;
            }
        }

        if (!found)
        {
            Console.WriteLine("Заметки не найдены.");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
        Console.ReadKey();
    }

    private string ReadMultiLineInput()
    {
        string input = "";
        string line;
        while ((line = Console.ReadLine()) != "")
        {
            input += line + Environment.NewLine;
        }
        return input.Trim();
    }

    private void LoadNotes()
    {
        if (File.Exists(dataFile))
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(dataFile, FileMode.Open)))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        Note note = new Note();
                        note.Title = reader.ReadString();
                        note.Content = reader.ReadString();
                        note.Created = DateTime.FromBinary(reader.ReadInt64());
                        if (reader.ReadBoolean())
                        {
                            note.Modified = DateTime.FromBinary(reader.ReadInt64());
                        }
                        notes.Add(note);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Ошибка при загрузке заметок. Будет создан новый файл.");
            }
        }
    }

    private void SaveNotes()
    {
        try
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(dataFile, FileMode.Create)))
            {
                writer.Write(notes.Count);
                foreach (Note note in notes)
                {
                    writer.Write(note.Title);
                    writer.Write(note.Content);
                    writer.Write(note.Created.ToBinary());
                    writer.Write(note.Modified.HasValue);
                    if (note.Modified.HasValue)
                    {
                        writer.Write(note.Modified.Value.ToBinary());
                    }
                }
            }
        }
        catch
        {
            Console.WriteLine("Ошибка при сохранении заметок.");
        }
    }
}