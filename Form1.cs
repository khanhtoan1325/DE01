using DE01.DATA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DE01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                using (Model1 model1 = new Model1())
                {
                    List<Lop> listLops = model1.Lops.ToList();
                    List<Sinhvien> ListSinhvien = model1.Sinhviens.ToList();
                    FillClassCombobox(listLops);
                    BindGrid(ListSinhvien);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }

            

        }
        private void FillClassCombobox(List<Lop> lops)
        {
            cmbLopHoc.DataSource = lops;
            cmbLopHoc.DisplayMember = "TenLop"; 
            cmbLopHoc.ValueMember = "MaLop";    
        }

        private void BindGrid(List<Sinhvien> sinhviens)
        {
            dataGridView1.Rows.Clear();
            foreach (Sinhvien student in sinhviens)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = student.MaSV;
                dataGridView1.Rows[index].Cells[1].Value = student.HotenSV;
                dataGridView1.Rows[index].Cells[2].Value = student.NgaySinh;
                dataGridView1.Rows[index].Cells[3].Value = student.Lop.TenLop; 
            }
        }
        private int GetSelectedRow(string maSV)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value?.ToString() == maSV)
                {
                    return i;
                }
            }
            return -1;
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                
                string maSV = txtMa.Text;
                string tenSV = txtHoTen.Text;
                DateTime ngaySinh;

                
                if (!DateTime.TryParse(dateTimePicker1.Text, out ngaySinh))
                {
                    MessageBox.Show("Vui lòng nhập ngày sinh hợp lệ.", "Thông báo");
                    return;
                }

                if (cmbLopHoc.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn lớp học.", "Thông báo");
                    return;
                }

                string maLop = cmbLopHoc.SelectedValue.ToString();

                
                if (GetSelectedRow(maSV) != -1)
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại.", "Thông báo");
                    return;
                }

                
                Sinhvien newSinhVien = new Sinhvien
                {
                    MaSV = maSV,
                    HotenSV = tenSV,
                    NgaySinh = ngaySinh, 
                    MaLop = maLop
                };

                
                using (Model1 model1 = new Model1())
                {
                    model1.Sinhviens.Add(newSinhVien);
                    model1.SaveChanges();
                }

                
                int rowIndex = dataGridView1.Rows.Add();
                dataGridView1.Rows[rowIndex].Cells[0].Value = newSinhVien.MaSV;
                dataGridView1.Rows[rowIndex].Cells[1].Value = newSinhVien.HotenSV;
                dataGridView1.Rows[rowIndex].Cells[2].Value = newSinhVien.NgaySinh.ToString("dd/MM/yyyy"); // Hiển thị ngày sinh
                dataGridView1.Rows[rowIndex].Cells[3].Value = cmbLopHoc.Text;

                
                txtMa.Clear();
                txtHoTen.Clear();
                DateTime selectedDate = dateTimePicker1.Value;
                cmbLopHoc.SelectedIndex = -1;

                MessageBox.Show("Thêm sinh viên thành công.", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo");
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            timTheoKhoa();
        }

        private void timTheoKhoa()
        {
            using (var dbContext = new Model1())
            {
                
                string searchID = txtTimKiem.Text;

                var results = dbContext.Sinhviens.AsQueryable();


                if (!string.IsNullOrEmpty(searchID))
                {
                    results = results.Where(s => s.MaSV.Contains(searchID));
                }

                var studentResults = results.ToList();

                if (studentResults.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sinh viên với tiêu chí đã chọn", "Thông báo", MessageBoxButtons.OK);
                    dataGridView1.DataSource = null;
                }
                else
                {
                    BindGrid(studentResults);
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                string maSV = txtMa.Text;

                
                if (string.IsNullOrWhiteSpace(maSV))
                {
                    MessageBox.Show("Vui lòng nhập mã sinh viên để xóa.", "Thông báo");
                    return;
                }

                int rowIndex = GetSelectedRow(maSV);

                
                if (rowIndex == -1)
                {
                    MessageBox.Show("Không tìm thấy sinh viên với mã đã nhập.", "Thông báo");
                    return;
                }

                
                using (Model1 model1 = new Model1())
                {
                    var studentToDelete = model1.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                    if (studentToDelete != null)
                    {
                        model1.Sinhviens.Remove(studentToDelete);
                        model1.SaveChanges();
                    }
                }

                
                dataGridView1.Rows.RemoveAt(rowIndex);
                MessageBox.Show("Xóa sinh viên thành công.", "Thông báo");

               
                txtMa.Clear();
                txtHoTen.Clear();
                cmbLopHoc.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                string maSV = txtMa.Text;

                
                if (string.IsNullOrWhiteSpace(maSV))
                {
                    MessageBox.Show("Vui lòng nhập mã sinh viên để sửa.", "Thông báo");
                    return;
                }

                int rowIndex = GetSelectedRow(maSV);

                
                if (rowIndex == -1)
                {
                    MessageBox.Show("Không tìm thấy sinh viên với mã đã nhập.", "Thông báo");
                    return;
                }

                
                string tenSV = txtHoTen.Text;
                DateTime ngaySinh = dateTimePicker1.Value;
                string maLop = cmbLopHoc.SelectedValue.ToString();

                
                using (Model1 model1 = new Model1())
                {
                    var studentToUpdate = model1.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);
                    if (studentToUpdate != null)
                    {
                        studentToUpdate.HotenSV = tenSV;
                        studentToUpdate.NgaySinh = ngaySinh;
                        studentToUpdate.MaLop = maLop;
                        model1.SaveChanges();
                    }
                }

                
                dataGridView1.Rows[rowIndex].Cells[1].Value = tenSV;
                dataGridView1.Rows[rowIndex].Cells[2].Value = ngaySinh.ToString("dd/MM/yyyy");
                dataGridView1.Rows[rowIndex].Cells[3].Value = cmbLopHoc.Text;

                MessageBox.Show("Cập nhật sinh viên thành công.", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo");
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn Đang Muốn thoát","Thông Báo",MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Close();
            } 
                
        }
        




    }
}
