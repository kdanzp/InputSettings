using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.Macros;

namespace InputSettings
{
    public static class LocalVariableExt
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Преобразует значение локальной переменной в логическое значение (bool).
        /// Выбрасывает FormatException, если преобразование не удалось.
        /// </summary>
        /// <returns>Логическое значение преобразованной переменной (bool).</returns>
        public static bool ToBool(this ILocalVariable variable)
        {
            if (!bool.TryParse(variable.Value, out bool result))
            {
                throw new FormatException($"Значение переменной \"{variable.Name}\" не может быть преобразовано в тип данных Bool.");
            }
            return result;
        }

        /// <summary>
        /// Преобразует значение локальной переменной в целочисленное значение (int).
        /// Выбрасывает FormatException, если преобразование не удалось.
        /// </summary>
        /// <returns>Целочисленное значение преобразованной переменной (int).</returns>
        public static int ToInt(this ILocalVariable variable)
        {
            return int.TryParse(variable.Value, out int result) ? result : 
                throw new FormatException($"Значение переменной \"{variable.Name}\" не может быть преобразовано в тип данных Int.");
        }

        /// <summary>
        /// Преобразует значение переменной с разделителями в массив строк.
        /// </summary>
        /// <param name="separator">Разделитель для разбиения строки (по умолчанию ',').</param>
        /// <returns>Строковый массив значений после разделения.</returns>
        public static string[] MultiSelectToArray(this ILocalVariable variable, char separator = ',')
        {
            var value = variable.Value;

            if (value.Contains(separator))
            {
                return value.Split(separator)
                      .Select(s => s.Trim())
                      .ToArray();
            }
            else return new string[] { value.Trim() };
        }

        /// <summary>
        /// Считывает значения из файла, каждую строку файла преобразует в элемент списка.
        /// </summary>
        /// <returns>Список строк, прочитанных из файла.</returns>
        public static List<string> ToList(this ILocalVariable variable)
        {
            return File.ReadAllLines(variable.Value).ToList();
        }

        /// <summary>
        /// Получает случайное целочисленное значение из заданного диапазона значений переменной, либо из максимального значения.
        /// Выбрасывает FormatException, если значение переменной не соответствует формату диапазона.
        /// </summary>
        /// <returns>Случайное целое число в заданном диапазоне или из максимального значения (int).</returns>
        public static int GetRandomInt(this ILocalVariable variable)
        {
            var values = variable.Value;

            if (values.Contains("-"))
            {
                var parts = values.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[0], out var minValue) && int.TryParse(parts[1], out var maxValue))
                {
                    return _random.Next(minValue, maxValue + 1);
                }
                else
                {
                    throw new FormatException($"Некорректный формат диапазона переменной \"{variable.Name}\"");
                }
            }
            else return _random.Next(variable.ToInt());
        }

        /// <summary>
        /// Обрабатывает текст с использованием спинтаксиса для создания различных комбинаций.
        /// </summary>
        /// <returns>Текст после обработки спинтаксиса.</returns>
        public static string Spintax(this ILocalVariable variable)
        {
            return TextProcessing.Spintax(variable.Value);
        }

        /// <summary>
        /// Проверяет значение переменной на наличие данных, выбрасывает исключение, если значение пустое.
        /// </summary>
        /// <param name="exceptionMessage">Сообщение исключения, если значение пустое (не обязательно).</param>
        /// <returns>Тот же экземпляр, для цепочки вызовов.</returns>
        public static ILocalVariable ExceptionEmpty(this ILocalVariable variable, string exceptionMessage = null)
        {
            if (string.IsNullOrWhiteSpace(variable.Value))
            {
                throw new Exception(exceptionMessage ?? $"Переменная \"{variable.Name}\" не имеет данных.");
            }
            return variable;
        }

        /// <summary>
        /// Проверяет существование файла и его содержимое.
        /// Выбрасывает исключение, если файл не найден или пустой, в зависимости от параметра ExceptionIfEmpty.
        /// </summary>
        /// <param name="ExceptionIfEmpty">Проверка на пустой файл.</param>
        /// <returns>Тот же экземпляр, для цепочки вызовов.</returns>
        public static ILocalVariable ExceptionFile(this ILocalVariable variable, bool ExceptionIfEmpty = true)
        {
            string message;
            var value = variable.Value;
            if (!File.Exists(value))
            {
                message = string.Format("Файл \"{0}\" не найден", value);
                throw new Exception(message);
            }

            if(ExceptionIfEmpty && new FileInfo(value).Length == 0)
            {
                message = string.Format("Файл \"{0}\" пустой", value);
                throw new Exception(message);
            }

            return variable;
        }

        /// <summary>
        /// Проверяет существование директории и её содержимое.
        /// Выбрасывает исключение, если директория не найдена или пустая, в зависимости от параметра ExceptionIfEmpty.
        /// </summary>
        /// <param name="ExceptionIfEmpty">Проверка на пустую директорию.</param>
        /// <returns>Тот же экземпляр, для цепочки вызовов.</returns>
        public static ILocalVariable ExceptionDirectory(this ILocalVariable variable, bool ExceptionIfEmpty = true)
        {
            string message;
            var value = variable.Value;
            if (!Directory.Exists(value))
            {
                message = string.Format("Директория \"{0}\" не найден", value);
                throw new Exception(message);
            }

            if (ExceptionIfEmpty && !Directory.EnumerateFileSystemEntries(value).Any())
            {
                message = string.Format("Директория \"{0}\" пустая", value);
                throw new Exception(message);
            }

            return variable;
        }
    }
}
