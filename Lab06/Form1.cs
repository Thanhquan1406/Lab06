using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab06.ViewModel;
using Lab06.Models;

namespace Lab06
{
    public partial class Form1 : Form
    {
        private DBContext db;
        private List<SachViewModel> sachs;
        private List<MaSachViewModel> masachs;
        public Form1()
        {
            InitializeComponent();
            db = new DBContext();
            LoadLoaiSach();
            FillDacultyComboBox(masachs);
            LoadBook();
        }
        private void LoadLoaiSach()
        {
            masachs = db.LoaiSaches.Select(s => new MaSachViewModel
            {
                TenLoai = s.TenLoai,
                MaLoai = s.MaLoai,
            }).ToList();
        }
        private void FillDacultyComboBox(List<MaSachViewModel> maSachViewModels)
        {
            cmbTheLoai.DataSource = maSachViewModels;
            cmbTheLoai.DisplayMember = "TenLoai";
            cmbTheLoai.ValueMember = "MaLoai";
        }
        private void LoadBook()
        {
            sachs = db.Saches.Select(s => new SachViewModel
            {
                MaSach = s.MaSach,
                TenSach = s.TenSach,
                NamXB = s.NamXB,
                TheLoai = s.LoaiSach.TenLoai

            }).ToList();
        }
        private void BinGird(List<SachViewModel> Saches)
        {
            dgvThuVien.DataSource = Saches;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BinGird(sachs);
        }

        private void dgvThuVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvThuVien.Rows[e.RowIndex];

                txtBookID.Text = selectedRow.Cells["MaSach"].Value?.ToString() ?? string.Empty;
                txtNameBook.Text = selectedRow.Cells["TenSach"].Value?.ToString() ?? string.Empty;
                txtYear.Text = selectedRow.Cells["NamXB"].Value?.ToString() ?? string.Empty;

                string selectedLoaiSach = selectedRow.Cells["TheLoai"].Value?.ToString() ?? string.Empty;
                cmbTheLoai.SelectedIndex = masachs.FindIndex(f => f.TenLoai == selectedLoaiSach);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtBookID.Text == "")
                    throw new Exception("Sách cần xóa không tồn tại!");
                var Sachcanxoa = db.Saches.FirstOrDefault(s=>s.MaSach == txtBookID.Text);
                if (Sachcanxoa == null)
                {
                    MessageBox.Show("Sách cần xóa không tồn tại!", "Thông Báo", MessageBoxButtons.OK);
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("Bạn có muốn xóa không?", "Xác Nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        db.Saches.Remove(Sachcanxoa);
                        db.SaveChanges();
                        LoadBook();
                        MessageBox.Show("Xóa thành công!", "Thông Báo", MessageBoxButtons.OK);
                        BinGird(sachs);
                        ResetData();
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void ResetData()
        {
            txtBookID.Text = "";
            txtNameBook.Text = "";
            txtYear.Text = "";
            cmbTheLoai.SelectedIndex = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtBookID.Text == "" || txtNameBook.Text == "" || txtYear.Text == "")
                    throw new Exception("Vui lòng nhập đầy đủ thông tin sách!");
                if (txtBookID.Text.Length != 6)
                    throw new Exception("Mã sách phải có 6 kí tự!");
                Sach sach = new Sach();
                sach.MaSach = txtBookID.Text;
                sach.TenSach = txtNameBook.Text;
                sach.NamXB = int.Parse(txtYear.Text);
                sach.MaLoai = (cmbTheLoai.SelectedItem as MaSachViewModel).MaLoai;

                db.Saches.Add(sach);
                db.SaveChanges();

                LoadBook();
                MessageBox.Show("Thêm mới thành công", "Thông Báo", MessageBoxButtons.OK);
                BinGird(sachs);
                ResetData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(txtBookID.Text) ||  string.IsNullOrWhiteSpace(txtNameBook.Text) || string.IsNullOrWhiteSpace(txtYear.Text))
                    throw new Exception("Vui lòng nhập đầy đủ thông tin sách!");
                if (txtBookID.Text.Length != 6)
                    throw new Exception("Mã sách phải có 6 kí tự!");
                var kiemtra = db.Saches.FirstOrDefault(s=>s.MaSach == txtBookID.Text);
                if (kiemtra == null)
                {
                    MessageBox.Show("Không tìm thấy sách cần sửa!", "Thông Báo", MessageBoxButtons.OK);
                    return;
                }

                kiemtra.TenSach = txtNameBook.Text;
                kiemtra.NamXB = int.Parse(txtYear.Text);
                kiemtra.MaLoai = (cmbTheLoai.SelectedItem as MaSachViewModel).MaLoai;

                db.SaveChanges();
                LoadBook();
                MessageBox.Show("Cập nhật sách thành công!", "Thông Báo", MessageBoxButtons.OK);
                BinGird(sachs);
                ResetData();
            }
            catch(Exception ex) 
            { 
                MessageBox.Show(ex.Message); 
            }
        }
        private void SearchBooks(string keyword)
        {
            var filteredSachs = sachs.Where(s =>
                s.MaSach.ToLower().Contains(keyword.ToLower()) ||
                s.TenSach.ToLower().Contains(keyword.ToLower()) ||
                s.NamXB.ToString().Contains(keyword)
            ).ToList();

            BinGird(filteredSachs);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            SearchBooks(keyword);
        }

        private void thốngKêTheoNămToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 reportForm = new Form2();
            reportForm.ShowDialog();
        }
    }
}
