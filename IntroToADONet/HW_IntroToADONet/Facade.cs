using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
namespace HW_IntroToADONet
{
    class Facade
    {
        static string connectString = ConfigurationManager.ConnectionStrings["defaultConnect"].ConnectionString;
        public static void MainMenu()
        {           
            ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
            string login, password, email, lName, fName;
            int authorization = 0; //принимает значение команды ExecuteNonQuery при авторизации и 
                                   //выходе из авторизации. Служит для проверки авторизации

            using (SqlConnection connection = new SqlConnection(connectString)) //выход из уч.записи, если юзер забыл
            {
                connection.Open();
                SqlCommand comm = new SqlCommand
                {
                    CommandText = "sp_EndAuthorization",
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                comm.ExecuteNonQuery();
            }
            do
            {
                Clear();
                WriteLine("1. Авторизация");
                WriteLine("2. Выход из аккаунта");
                WriteLine("3. Просмотр книг");
                WriteLine("4. Поиск книг");
                WriteLine("5. Добавить книгу");
                WriteLine("6. Удалить книгу");
                WriteLine("7. Обновить книгу");
                WriteLine("8. Добавить аккаунт");                
                WriteLine("9. Просмотр статистики");
                WriteLine("Esc выход из программы");
                keyInfo = ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1: //Авторизация
                        Clear();
                        if (!(authorization > 0))
                        {
                            using (SqlConnection connection = new SqlConnection(connectString))
                            {
                                connection.Open();
                                Write("Логин: ");
                                login = ReadLine();
                                Write("Пароль: ");
                                password = ReadLine();
                                SqlCommand comm = new SqlCommand
                                {
                                    CommandText = "sp_Authorization",
                                    Connection = connection,
                                    CommandType = CommandType.StoredProcedure
                                };

                                SqlParameter logParam = new SqlParameter
                                {
                                    ParameterName = "@login",
                                    Value = login,
                                    SqlDbType = SqlDbType.VarChar
                                };

                                SqlParameter pswdParam = new SqlParameter
                                {
                                    ParameterName = "@password",
                                    Value = password,
                                    SqlDbType = SqlDbType.VarChar
                                };

                                comm.Parameters.Add(logParam);
                                comm.Parameters.Add(pswdParam);
                                authorization = comm.ExecuteNonQuery();
                                if (authorization > 0)
                                {
                                    WriteLine("Вы авторизованы");
                                }
                                else
                                {
                                    WriteLine("Логин и/или пароль не совпадают");
                                }                                
                            }
                        }
                        else
                        {
                            WriteLine("Вы уже авторизованы");
                        }
                        ReadKey();
                        break;

                    case ConsoleKey.D2: //выход из аккаунта
                        Clear();
                        if (authorization > 0)
                        {
                            using (SqlConnection connection=new SqlConnection(connectString))
                            {
                                connection.Open();
                                SqlCommand comm = new SqlCommand
                                {
                                    CommandText="sp_EndAuthorization",
                                    Connection=connection,
                                    CommandType=CommandType.StoredProcedure
                                };
                                authorization = comm.ExecuteNonQuery();
                                WriteLine("Вы вышли из учетной записи");                              
                            }
                        }
                        else
                        {
                            WriteLine("Вы не авторизованы");
                        }
                        ReadKey();
                        break;

                    case ConsoleKey.D3: //просмотр книг
                        using(SqlConnection connection = new SqlConnection(connectString))
                        {
                            connection.Open();
                            const string query = "select NameBook,Price,Pages,DateOfPublish,LastName,FirstName from books, Authors where Books.id_author=Authors.id";
                            
                            SqlCommand comm = new SqlCommand(query, connection);
                            SqlDataReader reader = comm.ExecuteReader();
                            if (reader.HasRows)
                            {
                                Clear();
                                WriteLine("NameBook\tPrice\tPages\tDateOfPublish\tAuthor\n");
                                while (reader.Read())
                                {
                                    string NameBook = reader.GetString(0);
                                    decimal Price = reader.GetDecimal(1);
                                    int Pages = reader.GetInt32(2);
                                    string DateOfPublish = reader.GetDateTime(3).ToShortDateString();
                                    string LastName = reader.GetString(4);
                                    string FirstName = reader.GetString(5);
                                  
                                    WriteLine($"{NameBook}\t{Price}\t{Pages}\t{DateOfPublish}\t{LastName} {FirstName}");
                                    
                                }
                            }
                            else
                            {
                                WriteLine("В настоящее время книги отсутствуют");
                            }
                        }
                        ReadKey();
                        break;

                    case ConsoleKey.D4:
                        int selection = 0;

                        do
                        {
                            Clear();
                            WriteLine("1. Поиск по названию книги");
                            WriteLine("2. Поиск по автору");
                            WriteLine("3. Поиск по жанру");
                            selection = Int32.Parse(ReadLine());
                            if(selection>0 && selection < 4)
                            {
                                using (SqlConnection connection = new SqlConnection(connectString))
                                {
                                    connection.Open();
                                    string queryBooks = "select * from Books";
                                    string queryAuthors = "select NameBook, Price,LastName,FirstName from Books, Authors where Books.id_author=Authors.id";
                                    string queryCategory = "select NameBook, Price, NameCategory from Books,Category where Books.id_category=Category.id";
                                    string tmp;                                   
                                    switch (selection)
                                    {
                                        case 1:
                                            Clear();
                                            SqlCommand comm = new SqlCommand(queryBooks, connection);
                                            SqlDataReader reader = comm.ExecuteReader();
                                            Write("Введите название книги: ");
                                            tmp = ReadLine();
                                            if (reader.HasRows)
                                            {
                                                WriteLine("Книги, соответствующие названию: \n");
                                                while (reader.Read())
                                                {

                                                    if (reader.GetString(1) == tmp)
                                                    {
                                                        WriteLine($"{reader.GetString(1)}\t{reader.GetDecimal(2)}\t{reader.GetInt32(3)}");
                                                    }
                                                }
                                            }
                                            ReadKey();
                                            break;

                                        case 2:
                                            Clear();
                                            SqlCommand comm2 = new SqlCommand(queryAuthors, connection);
                                            SqlDataReader reader2 = comm2.ExecuteReader();
                                            Write("Введите фамилию автора: ");
                                            tmp = ReadLine();
                                            if (reader2.HasRows)
                                            {
                                                WriteLine("Книги, соответствующие автору: \n");
                                                while (reader2.Read())
                                                {

                                                    if (reader2.GetString(2) == tmp)
                                                    {
                                                        WriteLine($"{reader2.GetString(0)}\t{reader2.GetDecimal(1)}\t{reader2.GetString(2)}\t{reader2.GetString(3)}");
                                                    }
                                                }
                                            }
                                            ReadKey();
                                            break;

                                        case 3:
                                            Clear();
                                            SqlCommand comm3 = new SqlCommand(queryCategory, connection);
                                            SqlDataReader reader3 = comm3.ExecuteReader();
                                            Write("Введите жанр: ");
                                            tmp = ReadLine();
                                            if (reader3.HasRows)
                                            {
                                                WriteLine("Книги, соответствующие жанру: \n");
                                                while (reader3.Read())
                                                {
                                                    if (reader3.GetString(2) == tmp)
                                                    {
                                                        WriteLine($"{reader3.GetString(0)}\t{reader3.GetDecimal(1)}\t{reader3.GetString(2)}");
                                                    }
                                                }
                                            }
                                            ReadKey();
                                            break;

                                    }
                                    

                                }
                            }
                            
                        } while (selection < 1 || selection > 3);
                        break;

                    case ConsoleKey.D5: //добавить книгу
                        Clear();
                        if (authorization > 0) //проверка авторизации
                        {                                                                                                                               
                            Write("Название книги: ");
                            string NameBook = ReadLine();
                            Write("Фамилия автора: ");
                            string LastName = ReadLine();
                            Write("Имя автора: ");
                            string FirstName = ReadLine();
                            Write("Страна проживания автора: ");
                            string CountryAuthor = ReadLine();
                            Write("Жанр книги: ");
                            string Category = ReadLine();
                            Write("Цена книги: ");
                            decimal price = decimal.Parse(ReadLine());
                            Write("Количество страниц: ");
                            int pages = Int32.Parse(ReadLine());
                            Write("количество экземпляров книги: ");
                            int quantity = Int32.Parse(ReadLine());
                            Write("Дата публикации, например (1990-10-20): ");
                            DateTime date = DateTime.Parse(ReadLine());
                            using (SqlConnection connection=new SqlConnection(connectString))
                            {
                                connection.Open();
                                SqlCommand comm = new SqlCommand
                                {
                                    CommandText = "sp_AddBookDanger",
                                    Connection=connection,
                                    CommandType=CommandType.StoredProcedure
                                };
                                SqlParameter NameBookParam = new SqlParameter
                                {
                                    ParameterName = "@NameBook",
                                    SqlDbType = SqlDbType.NVarChar,
                                    Value = NameBook
                                };
                                SqlParameter LNameParam = new SqlParameter
                                {
                                    ParameterName = "@AuthorLastName",
                                    SqlDbType = SqlDbType.NVarChar,
                                    Value = LastName
                                };
                                SqlParameter FNameParam = new SqlParameter
                                {
                                    ParameterName = "@AuthorFirstName",
                                    SqlDbType = SqlDbType.NVarChar,
                                    Value = FirstName
                                };
                                SqlParameter countryParam = new SqlParameter
                                {
                                    ParameterName = "@NameCountryAuthor",
                                    SqlDbType = SqlDbType.NVarChar,
                                    Value = CountryAuthor
                                };
                                SqlParameter categoryParam = new SqlParameter
                                {
                                    ParameterName = "@NameCategory",
                                    SqlDbType = SqlDbType.NVarChar,
                                    Value = Category
                                };
                                SqlParameter priceParam = new SqlParameter
                                {
                                    ParameterName = "@price",
                                    SqlDbType = SqlDbType.Decimal,
                                    Value = price
                                };
                                SqlParameter pagesParam = new SqlParameter
                                {
                                    ParameterName = "@pages",
                                    SqlDbType = SqlDbType.Int,
                                    Value = pages
                                };
                                SqlParameter quantityParam = new SqlParameter
                                {
                                    ParameterName = "@quantity",
                                    SqlDbType = SqlDbType.Int,
                                    Value = quantity
                                };
                                SqlParameter dateParam = new SqlParameter
                                {
                                    ParameterName = "@DateOfPublish",
                                    SqlDbType = SqlDbType.Date,
                                    Value = date
                                };
                                comm.Parameters.Add(NameBookParam);
                                comm.Parameters.Add(LNameParam);
                                comm.Parameters.Add(FNameParam);
                                comm.Parameters.Add(countryParam);
                                comm.Parameters.Add(categoryParam);
                                comm.Parameters.Add(priceParam);
                                comm.Parameters.Add(pagesParam);
                                comm.Parameters.Add(quantityParam);
                                comm.Parameters.Add(dateParam);
                                var tmp = comm.ExecuteNonQuery();
                                if (tmp > 0)
                                {
                                    WriteLine("Книга добавлена");
                                }
                                else
                                {
                                    WriteLine("Ошибка! Книга не добавлена");
                                }
                            }
                        }
                        else
                        {
                            WriteLine("Добавлять книги могут только авторизованные пользователи");
                        }
                        ReadKey();
                        break;

                    case ConsoleKey.D6: //удалить книгу
                        //Этот пункт работает, если убрать все зависимости в БД. У меня в БД очень много зависимостей,
                        //не позволяющих делать удаление
                        Clear();
                        if (authorization > 0)
                        {
                            Write("Название книги: ");
                            string NameBook = ReadLine();
                            using(SqlConnection connection = new SqlConnection(connectString))
                            {
                                connection.Open();
                                SqlCommand comm = new SqlCommand
                                {
                                    CommandText="sp_DelBook",
                                    CommandType=CommandType.StoredProcedure,
                                    Connection=connection
                                };
                                SqlParameter NameBookParam = new SqlParameter
                                {
                                    ParameterName = "@NameBook",
                                    SqlDbType = SqlDbType.NVarChar,
                                    Value = NameBook
                                };
                                comm.Parameters.Add(NameBookParam);
                                var tmp = comm.ExecuteNonQuery();
                                if (tmp > 0)
                                {
                                    WriteLine("Книга была удалена");
                                }
                                else
                                {
                                    WriteLine("Ошибка! Книга не удалена");
                                }
                            }
                        }
                        else
                        {
                            WriteLine("Удалять книги могут только авторизованные пользователи");
                        }
                        ReadKey();
                        break;

                    case ConsoleKey.D7: //изменение названия книги
                        Clear();
                        if (authorization > 0)
                        {
                            Write("Старое название книги: ");
                            string oldName = ReadLine();
                            Write("Новое название книги: ");
                            string newName = ReadLine();
                            using(SqlConnection connection=new SqlConnection(connectString))
                            {
                                connection.Open();
                                SqlCommand comm = new SqlCommand
                                {
                                    CommandText = "sp_UpdateBook",
                                    CommandType = CommandType.StoredProcedure,
                                    Connection = connection
                                };
                                SqlParameter OldNameParam = new SqlParameter
                                {
                                    ParameterName = "@NameBookOld",
                                    SqlDbType = SqlDbType.NVarChar,
                                    Value = oldName
                                };
                                comm.Parameters.Add(OldNameParam);
                                SqlParameter NewNameParam = new SqlParameter
                                {
                                    ParameterName = "@NameBookNew",
                                    SqlDbType = SqlDbType.NVarChar,
                                    Value = newName
                                };
                                comm.Parameters.Add(NewNameParam);
                                var tmp = comm.ExecuteNonQuery();
                                if (tmp > 0)
                                {
                                    WriteLine("Название книги изменено");
                                }
                                else
                                {
                                    WriteLine("Ошибка! Название не изменено!");                                   
                                }
                            }
                        }
                        else
                        {
                            WriteLine("Только авторизованные пользователи могут изменять книги");
                        }
                        ReadKey();
                        break;

                    case ConsoleKey.D8: //Добавление аккаунта
                        Clear();
                        Write("Логин: ");
                        login = ReadLine();
                        Write("Пароль: ");
                        password = ReadLine();
                        Write("Фамилия: ");
                        fName = ReadLine();
                        Write("Имя: ");
                        lName = ReadLine();
                        Write("email: ");
                        email = ReadLine();
                        using(SqlConnection connection=new SqlConnection(connectString))
                        {
                            connection.Open();
                            SqlCommand comm = new SqlCommand
                            {
                                CommandText = "sp_AddAccount",
                                Connection = connection,
                                CommandType = CommandType.StoredProcedure
                            };

                            SqlParameter logParam = new SqlParameter
                            {
                                ParameterName="@login",
                                Value=login,
                                SqlDbType=SqlDbType.VarChar
                            };

                            SqlParameter pswdParam = new SqlParameter
                            {
                                ParameterName="@password",
                                Value=password,
                                SqlDbType=SqlDbType.VarChar,
                            };

                            SqlParameter emailParam = new SqlParameter
                            {
                                ParameterName = "@email",
                                Value = email,
                                SqlDbType = SqlDbType.VarChar
                            };

                            SqlParameter lNameParam = new SqlParameter
                            {
                                ParameterName = "@LastName",
                                Value = lName,
                                SqlDbType = SqlDbType.NVarChar
                            };

                            SqlParameter fNameParam = new SqlParameter
                            {
                                ParameterName = "@FirstName",
                                Value = fName,
                                SqlDbType = SqlDbType.NVarChar
                            };

                            comm.Parameters.Add(logParam);
                            comm.Parameters.Add(pswdParam);
                            comm.Parameters.Add(emailParam);
                            comm.Parameters.Add(lNameParam);
                            comm.Parameters.Add(fNameParam);

                            int tmp = comm.ExecuteNonQuery();
                            if (tmp > 0)
                            {
                                WriteLine("Пользователь добавлен");
                            }
                            else
                            {
                                WriteLine("Ошибка! Пользователь не добавлен");
                            }                          
                            ReadKey();
                        }
                        break;

                    case ConsoleKey.D9: //просмотр статистики
                        Clear();
                        if (authorization > 0)
                        {
                            using(SqlConnection connection = new SqlConnection(connectString))
                            {
                                connection.Open();
                                const string JournalInsertedStr = "select NameBook from (select id_book, JournalBooksInserted.id_account from JournalBooksInserted,CurrentAccount where CurrentAccount.id_account=JournalBooksInserted.id_account) as tt,Books where id_book=id";
                                SqlCommand command = new SqlCommand(JournalInsertedStr, connection);
                                SqlDataReader reader = command.ExecuteReader();
                                if (reader.HasRows)
                                {
                                    WriteLine("Вами были добавлены книги: \n");
                                    while (reader.Read())
                                    {
                                        WriteLine($"{reader.GetValue(0).ToString()}");
                                    }
                                }
                                WriteLine();
                                reader.Close();
                                const string JournalUpdateStr = "select NameBook from (select id_account from CurrentAccount) as tt,JournalBookUpdate where tt.id_account=JournalBookUpdate.id_account";
                                command = new SqlCommand(JournalUpdateStr, connection);
                                reader = command.ExecuteReader();
                                if (reader.HasRows)
                                {
                                    WriteLine("Вами были изменены названия книг: \n");
                                    while (reader.Read())
                                    {
                                        WriteLine($"{reader.GetValue(0).ToString()}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            WriteLine("Только авторизованные пользователи могут просматривать статистику");
                        }
                        ReadKey();
                        break;
                }
            } while (keyInfo.Key != ConsoleKey.Escape);
            using (SqlConnection connection = new SqlConnection(connectString)) //выход из уч.записи, если юзер забыл
            {
                connection.Open();
                SqlCommand comm = new SqlCommand
                {
                    CommandText = "sp_EndAuthorization",
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };
                comm.ExecuteNonQuery();
            }
            
        }
    }
}
