using Lab06.Models;
using Lab06.ViewModel;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06
{
    public partial class Form2 : Form
    {
        private DBContext db;
        private List<SachViewModel> sachs;
        public Form2()
        {
            InitializeComponent();
            db = new DBContext();
            LoadReport();
        }
        private void LoadReport()
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            sachs = db.Saches.Select(s => new SachViewModel
            {
                MaSach = s.MaSach,
                TenSach = s.TenSach,
                NamXB = s.NamXB,
                TheLoai = s.LoaiSach.TenLoai,
            }).OrderByDescending(s => s.NamXB).ToList();

            reportViewer1.LocalReport.ReportPath = "ReportBooks.rdlc";
            var reportDataSource = new ReportDataSource("DataSet1", sachs);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.RefreshReport();

        }


        private void Form2_Load(object sender, EventArgs e)
        {
            LoadReport();
        }
    }
}
