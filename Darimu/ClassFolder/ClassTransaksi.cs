﻿using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Darimu.ClassFolder
{
    class ClassTransaksi
    {
        static SqlConnection sqlcon = new ClassKoneksi().getSQLCon();
        public static long get_saldo(string id_pengguna)
        {
            long ambil_saldo = 0;

            sqlcon.Open();
            SqlCommand sqlcom = new SqlCommand("SELECT saldo FROM tb_pengguna WHERE id_pengguna = '" + id_pengguna + "'", sqlcon);
            SqlDataReader dr = sqlcom.ExecuteReader();

            if (dr.Read())
            {
                ambil_saldo = dr.GetInt64(0);
            }

            sqlcon.Close();
            return ambil_saldo;
        }

        public static long isi_saldo(string id_pengguna, long saldo, string keterangan)
        {
            long saldo_baru = 0;
            saldo_baru = saldo + get_saldo(id_pengguna);
            SqlCommand sqlcom;

            sqlcon.Open();
            sqlcom = new SqlCommand("UPDATE tb_pengguna SET saldo = '" + saldo_baru + "' WHERE id_pengguna = '" + id_pengguna + "'", sqlcon);
            sqlcom.ExecuteNonQuery();
            sqlcon.Close();

            sqlcon.Open();
            sqlcom = new SqlCommand("UPDATE tb_transaksi SET keterangan = '" + keterangan + "' WHERE id_transaksi = (SELECT TOP 1 id_transaksi FROM tb_transaksi WHERE id_pengguna = '" + id_pengguna + "' ORDER BY tanggal DESC);", sqlcon);
            sqlcom.ExecuteNonQuery();
            sqlcon.Close();

            return saldo_baru;
        }

        public static long isi_saldo_impian(string id_pengguna, long isi_saldo_tabungan_impian, long saldo_terkumpul, long saldo_impian, string keterangan)
        {
            long saldo_lama = 0;
            long saldo_baru = 0;
            saldo_lama = get_saldo(id_pengguna);
            saldo_baru = saldo_lama - isi_saldo_tabungan_impian;

            if (saldo_impian < saldo_terkumpul)
            {
                saldo_baru = saldo_lama;
                return saldo_baru;
            }
            else if (saldo_baru < 0)
            {
                return saldo_baru;
            }
            else
            {
                SqlCommand sqlcom;

                sqlcon.Open();
                sqlcom = new SqlCommand("UPDATE tb_pengguna SET saldo = '" + saldo_baru + "' WHERE id_pengguna = '" + id_pengguna + "'", sqlcon);
                sqlcom.ExecuteNonQuery();
                sqlcon.Close();

                sqlcon.Open();
                sqlcom = new SqlCommand("UPDATE tb_transaksi SET keterangan = '" + keterangan + "' WHERE id_transaksi = (SELECT TOP 1 id_transaksi FROM tb_transaksi WHERE id_pengguna = '" + id_pengguna + "' ORDER BY tanggal DESC);", sqlcon);
                sqlcom.ExecuteNonQuery();
                sqlcon.Close();
                return saldo_baru;
            }
        }

        public static void riwayat_transaksi(string id_pengguna, DataGridView grid_transaksi)
        {
            grid_transaksi.Rows.Clear();
            sqlcon.Open();
            SqlCommand sqlcom = new SqlCommand("SELECT * FROM view_transaksi WHERE [ID PENGGUNA] = '" + id_pengguna + "'", sqlcon);
            SqlDataReader dr = sqlcom.ExecuteReader();
            while (dr.Read())
            {
                string tanggal = dr.GetDateTime(1).ToString();
                string keterangan = dr.GetString(2);
                string debit = dr.GetInt64(3).ToString();
                string kredit = dr.GetInt64(4).ToString();
                string saldo = dr.GetInt64(5).ToString();
                grid_transaksi.Rows.Add(tanggal, keterangan, debit, kredit, saldo);
            }
            sqlcon.Close();
        }
    }
}
