using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
public class NetworkManager : MonoBehaviour
{
    private static TcpClient client;
    private static NetworkStream stream;
    private static bool Connect() {
        try {
            NetworkManager.client = new TcpClient("192.168.1.54", 54429);
            NetworkManager.stream = client.GetStream();
            return true;
        } catch (Exception e) {
            Debug.LogError("Socket error: " + e.Message);
            return false;
        }
    }
    async public static void Login(string username, string password) {
        bool flag = NetworkManager.Connect();
        if (flag == true) {
            string message = "LOGIN " + username + " " + password + "\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await NetworkManager.stream.WriteAsync(data, 0, data.Length);
            Update1();
        }
    }
    public static void Register(string username, string password) {
        bool flag = NetworkManager.Connect();
        if (flag == true) {
            string message = "REGISTER " + username + " " + password + "\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            NetworkManager.stream.Write(data, 0, data.Length);
        }
    }
    public static void Fetch(string username) {
        bool flag = NetworkManager.Connect();
        if (flag == true) {
            string message = "FETCHSTATS " + username + "\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            NetworkManager.stream.Write(data, 0, data.Length);
        }
    }
    async public static void FetchStars(string username) {
        bool flag = NetworkManager.Connect();
        if (flag == true) {
            string message = "FETCHSTARS " + username + "\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await NetworkManager.stream.WriteAsync(data, 0, data.Length);
            Update1();
        }
    }
    async public static void FetchEndurance(string username) {
        bool flag = NetworkManager.Connect();
        if (flag == true) {
            string message = "FETCHENDURANCE " + username + "\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await NetworkManager.stream.WriteAsync(data, 0, data.Length);
            Update1();
        }
    }
    async public static void FetchSessions(string username) {
        bool flag = NetworkManager.Connect();
        if (flag == true) {
            string message = "FETCHSESSIONS " + username + "\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await NetworkManager.stream.WriteAsync(data, 0, data.Length);
            Update1();
        }
    }
    public static void GenerateItem(string username) {
        bool flag = NetworkManager.Connect();
        if (flag == true) {
            string message = "CREATEITEM " + username + "\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            NetworkManager.stream.Write(data, 0, data.Length);
        }
    }

    // async private static void PlayerInit() {
    //     NetworkManager.Fetch(temp[2]);
    //     NetworkManager.FetchStars(temp[2]);
    //     NetworkManager.FetchEndurance(temp[2]);
    //     NetworkManager.FetchSessions(temp[2]);
    // }

    private static void Update1() /* future rework of this method */ {
        if (stream != null && stream.DataAvailable) {
            byte[] receivedData = new byte[client.Available];
            NetworkManager.stream.Read(receivedData, 0, receivedData.Length);
            string message = Encoding.UTF8.GetString(receivedData);
            string[] temp = message.Split(' ');
            if (message.Contains("LOGIN 0") && GameManager.current_scene == "00_Login") {
                // NetworkManager.Fetch(temp[2]);
                NetworkManager.FetchStars(temp[2]);
                NetworkManager.FetchEndurance(temp[2]);
                NetworkManager.FetchSessions(temp[2]);
                PlayerManager.Set(temp[2]);
                ItemManager.Reset();
                GameManager.ChangeScene("01_Profile");
            } else if (message.Contains("REGISTER 0") && GameManager.current_scene == "00_Register") {
                GameManager.ChangeScene("00_Login");
            } else if (message.Contains("FETCHSTARS 0")) {
                Debug.Log("FETCHSTARS");
                PlayerManager.SetStars(int.Parse(temp[2]));
            }
            else if (message.Contains("FETCHENDURANCE 0")) {
                PlayerManager.SetEndurance(int.Parse(temp[2]));
                Debug.Log("FETCHENDURANCE");
            }
            else if (message.Contains("FETCHSESSIONS 0")) {
                Debug.Log("FETCHSESSIONS");
                PlayerManager.SetSessions(int.Parse(temp[2]));
            }
            else if (message.Contains("FETCHSTATS 0")) {
                Debug.Log("FETCHSTATS");
                temp = temp[2].Split('\t');
                PlayerManager.Set(0, int.Parse(temp[0])); PlayerManager.Set(1, int.Parse(temp[1])); PlayerManager.Set(2, int.Parse(temp[2]));
            }
            else if (message.Contains("CREATEITEM 0")) {
                temp = temp[2].Split('\t');
                string name = temp[0]; int trait = int.Parse(temp[1].Split(',')[0]); int value = int.Parse(temp[1].Split(',')[1]);
                if (trait == 0) {
                    ItemManager.locker[0] = new Item(name, value, 0 , 0);
                } else if (trait == 1) {
                    ItemManager.locker[0] = new Item(name, 0, value , 0);
                } else if (trait == 2) {
                    ItemManager.locker[0] = new Item(name, 0, 0 , value);
                }
                for (int m = 0; m < ItemManager.locker.Length; m++) {
                    if (ItemManager.locker[m] != null) {Debug.Log("Locker item " + m + ": " + ItemManager.locker[m].ToString());}
                }
            }
        }
    }
}