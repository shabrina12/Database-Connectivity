﻿using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows.Input;

namespace DatabaseConnectivity;

class Program
{
    static string ConnectionString = "Data Source=DESKTOP-I3RV54S;Initial Catalog=db_hr_sibkm;Integrated Security=True;Connect Timeout=30;";

    static SqlConnection connection;

    static void Main(string[] args)
    {

        try
        {
            connection.Open();
            Console.WriteLine("Koneksi Berhasil Dibuka!");
            connection.Close();
        }
        catch (Exception e) //NullReferenceException e
        {
            Console.WriteLine(e.Message);
        }

        int choice;
        string confirm;
        int id;
        string name;

        do
        {
            displaymenu();
            Console.Write("Input (1-5): ");
            choice = int.Parse(Console.ReadLine());
            Console.Clear();
            
            switch (choice)
            {
                case 1:
                    GetAllRegion();
                    break;
                case 2:
                    Console.Write("input id yang ingin dicari: ");
                    id = int.Parse(Console.ReadLine());
                    Console.WriteLine("========================");
                    GetRegionById(id);
                    break;
                case 3:
                    Console.Write("input nama region yang ingin ditambahkan: ");
                    name = Console.ReadLine();
                    InsertRegion(name);
                    break;
                case 4:
                    Console.Write("input id yang ingin diubah: ");
                    id = int.Parse(Console.ReadLine());
                    Console.Write("input nama region yang baru: ");
                    name = Console.ReadLine();
                    UpdateRegion(id, name);
                    break;
                case 5:
                    Console.Write("input id yang ingin dihapus: ");
                    id = int.Parse(Console.ReadLine());
                    DeleteRegion(id);
                    break;
                default:
                    Console.WriteLine("invalid input");
                    break;
            }
            Console.Write("Press y or Y to continue: ");
            confirm = Console.ReadLine().ToString();
            Console.Clear();
        } while (confirm == "y" || confirm == "Y");
    }

    // DISPLAY MENU 
    static void displaymenu()
    {
        Console.WriteLine("=========== MENU ===========");
        Console.WriteLine(" 1. Get All Region");
        Console.WriteLine(" 2. Get Region By Id");
        Console.WriteLine(" 3. Insert New Region");
        Console.WriteLine(" 4. Update Region by Id");
        Console.WriteLine(" 5. Delete Region");
    }

    // GETALL : REGION (Command)
    public static void GetAllRegion()
    {
        connection = new SqlConnection(ConnectionString);

        //Membuat instance untuk command
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM region";

        //Membuka koneksi
        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                Console.WriteLine("Id: " + reader[0]);
                Console.WriteLine("Name: " + reader[1]);
                Console.WriteLine("====================");
            }
        }
        else
        {
            Console.WriteLine("Data not found!");
        }
        reader.Close();
        connection.Close();
    }

    // GETBYID : REGION (Command)
    public static void GetRegionById(int id)
    {
        connection = new SqlConnection(ConnectionString);        

        //Membuat instance untuk command
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM region WHERE id = @id";
        command.Parameters.Add(new SqlParameter("id", id));

        //Membuka koneksi
        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                Console.WriteLine("Id: " + reader[0]);
                Console.WriteLine("Name: " + reader[1]);
                Console.WriteLine("====================");
            }
        }
        else
        {
            Console.WriteLine("Data not found!");
        }
        reader.Close();
        connection.Close();
    }

    // INSERT : REGION (Transaction)
    public static void InsertRegion(string name)
    {
        connection = new SqlConnection(ConnectionString);

        //Membuka koneksi
        connection.Open();

        SqlTransaction transaction = connection.BeginTransaction();
        try
        {
            //Membuat instance untuk command
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "INSERT INTO region (name) VALUES (@name)";
            command.Transaction = transaction;

            //Membuat parameter
            SqlParameter pName = new SqlParameter();
            pName.ParameterName = "@name";
            pName.Value = name;
            pName.SqlDbType = SqlDbType.VarChar;

            //Menambahkan parameter ke command
            command.Parameters.Add(pName);

            //Menjalankan command
            int result = command.ExecuteNonQuery();
            transaction.Commit();

            if (result > 0)
            {
                Console.WriteLine("Data berhasil ditambahkan!");
            }
            else
            {
                Console.WriteLine("Data gagal ditambahkan!");
            }

            //Menutup koneksi
            connection.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            try
            {
                transaction.Rollback();
            }
            catch (Exception rollback)
            {
                Console.WriteLine(rollback.Message);
            }
        }

    }

    // UPDATE : REGION (Transaction)
    public static void UpdateRegion(int id, string name)
    {
        connection = new SqlConnection(ConnectionString);

        //Membuka koneksi
        connection.Open();

        SqlTransaction transaction = connection.BeginTransaction();
        
        try
        {
            //Membuat instance untuk command
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "UPDATE region SET Name = @name WHERE id = @id";
            command.Transaction = transaction;

            //Membuat parameter
            SqlParameter pName = new SqlParameter();           
            pName.ParameterName = "@name";
            pName.Value = name;
            pName.SqlDbType = SqlDbType.VarChar;

            SqlParameter pId = new SqlParameter();
            pId.ParameterName = "@id";
            pId.Value = id;
            pId.SqlDbType = SqlDbType.Int;

            //Menambahkan parameter ke command
            command.Parameters.Add(pId);
            command.Parameters.Add(pName);

            //Menjalankan command
            int result = command.ExecuteNonQuery();
            transaction.Commit();

            if (result > 0)
            {
                Console.WriteLine("Data berhasil diubah!");
            }
            else
            {
                Console.WriteLine("Data gagal diubah!");
            }

            //Menutup koneksi
            connection.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            try
            {
                transaction.Rollback();
            }
            catch (Exception rollback)
            {
                Console.WriteLine(rollback.Message);
            }
        }

    }

    // DELETE : REGION (Transaction)
    public static void DeleteRegion(int id)
    {
        connection = new SqlConnection(ConnectionString);        
        connection.Open();
      
        SqlTransaction transaction = connection.BeginTransaction();
        
        try
        {
            //Membuat instance untuk command
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = "DELETE FROM region WHERE id = @id";
            command.Transaction = transaction;

            //Membuat parameter
            SqlParameter pId = new SqlParameter();
            pId.ParameterName = "@id";
            pId.Value = id;
            pId.SqlDbType = SqlDbType.Int;

            //Menambahkan parameter ke command
            command.Parameters.Add(pId);

            //Menjalankan command
            int result = command.ExecuteNonQuery();
            transaction.Commit();

            if (result > 0)
            {
                Console.WriteLine("Data berhasil dihapus!");
            }
            else
            {
                Console.WriteLine("Data gagal dihapus!");
            }

            //Menutup koneksi
            connection.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            try
            {
                transaction.Rollback();
            }
            catch (Exception rollback)
            {
                Console.WriteLine(rollback.Message);
            }
        }       
    }
}