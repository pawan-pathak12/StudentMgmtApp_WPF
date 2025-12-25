using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StudentManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region varialble
        private static readonly string PlaceholderText = "MM/DD/YYYY";
        SqlConnection con = new SqlConnection();
        SqlConnection connection = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter adapter = new SqlDataAdapter();
        int selectedStudentId;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            mycon();
            GetStudentData();
            DOB_TextBox.Text = PlaceholderText;
            GetCourseData();
        }
        #region Connection to databaase
        public void mycon()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["StudentManagementSystem.Properties.Settings.StudentManagementConnectionString"].ConnectionString;
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion
        #region DataViewerTab
        #region Data Binding From database to  VeiwerTab
        public void GetStudentData()
        {
            try
            {
                mycon();
                string query = "Select * from Students";
                cmd = new SqlCommand(query, connection);

                DataTable dataTable = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dataTable);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    DataViewerList.ItemsSource = dataTable.DefaultView;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        #endregion
        #endregion
        #region cursor Control
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MovetoNextControl(sender as UIElement);
            }

        }
        private void MovetoNextControl(UIElement currentElement)
        {
            currentElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
        #endregion
        #region Data Viewer Tap

        private void DataViewerList_SelectionChanges(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (DataViewerList.SelectedItems != null)
                {
                    DataRowView row = (DataRowView)DataViewerList.SelectedItem;
                    selectedStudentId = Convert.ToInt32(row["Id"]);
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString());
            }
        }

        private void DeleteStudent(int selectedId)
        {

            if (MessageBox.Show("Are you Sure you want to delete ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                try
                {
                    mycon();
                    string query = "delete from Student where Id=@Id";
                    cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id", selectedId);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    connection.Close();
                    MessageBox.Show("Student Record Deleted Successfully");
                    GetStudentData();
                }
            }

        }
        private void Delete_CLick(object sender, MouseButtonEventArgs e)
        {
            var row = (DataRowView)((FrameworkElement)sender).DataContext;
            if (row != null)
            {
                int selectedId = Convert.ToInt32(row["Id"]);
                DeleteStudent(selectedId);


            }
        }
        private void Edit_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (selectedStudentId > 0)
                {
                    MainTabControl.SelectedIndex = 1;
                    StudentAdderLabel.Content = "Edit Student Data";
                    AddButton.Content = "Update";

                    var row = (DataRowView)((FrameworkElement)sender).DataContext;
                    FirstName_Textbox.Text = row["FirstName"].ToString();
                    SecondName_Textbox.Text = row["LastName"].ToString();
                    Address_Textbox.Text = row["Address"].ToString();
                    Email_TextBox.Text = row["EmailAddress"].ToString();
                    DOB_TextBox.Text = row["DOB"].ToString();
                    FatherName_Textbox.Text = row["FatherName"].ToString();
                    MotherName_Textbox.Text = row["MotherName"].ToString();
                    Occupation_Textbox.Text = row["Occupation"].ToString();
                    PhoneNumber_TextBox.Text = row["PhoneNumber"].ToString();
                    Courses_TextBox.Text = row["Courses"].ToString();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }
        private void Refresh_Button(object sender, RoutedEventArgs e)
        {
            GetStudentData();

        }
        #endregion
        #region Student Data Entry Tab
        #region Student Adder tap CLick Event
        public void ClearData()
        {
            FirstName_Textbox.Text = string.Empty;
            SecondName_Textbox.Text = string.Empty;
            Address_Textbox.Text = string.Empty;
            Email_TextBox.Text = string.Empty;
            DOB_TextBox.Text = string.Empty;
            FatherName_Textbox.Text = string.Empty;
            MotherName_Textbox.Text = string.Empty;
            Occupation_Textbox.Text = string.Empty;
            PhoneNumber_TextBox.Text = string.Empty;
            Courses_TextBox.Text = string.Empty;
            FirstName_Textbox.Focus();

        }
        private void Cancel_Button(object sender, RoutedEventArgs e)
        {
            ClearData();
            if (AddButton.Content.ToString() == "Update")
            {
                MainTabControl.SelectedIndex = 0;
                AddButton.Content = "Add";
                StudentAdderLabel.Content = "Student Adder";
            }

        }
        private void AddOrUpdate_Button(object sender, RoutedEventArgs e)
        {
            if (AddButton.Content.ToString() == "Add")
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(FirstName_Textbox.Text))
                    {
                        MessageBox.Show("Please enter the first name.");
                        return;
                    }
                    else if (string.IsNullOrWhiteSpace(SecondName_Textbox.Text))
                    {
                        MessageBox.Show("Please enter the second name.");
                        return;
                    }
                    else if (string.IsNullOrWhiteSpace(Address_Textbox.Text))
                    {
                        MessageBox.Show("Please enter the Address.");
                        return;
                    }
                    else if (string.IsNullOrWhiteSpace(DOB_TextBox.Text))
                    {
                        MessageBox.Show("Please enter the DOB.");
                        return;
                    }
                    else if (string.IsNullOrWhiteSpace(FatherName_Textbox.Text))
                    {
                        MessageBox.Show("Please enter the Father name.");
                        return;
                    }
                    else if (string.IsNullOrWhiteSpace(MotherName_Textbox.Text))
                    {
                        MessageBox.Show("Please enter the Mother name.");
                        return;
                    }
                    else if (string.IsNullOrWhiteSpace(PhoneNumber_TextBox.Text))
                    {
                        MessageBox.Show("Please enter the Phone Number .");
                        return;
                    }

                    else
                    {
                        if (MessageBox.Show("Are you Sure do want to save ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            string query = "Insert into Students(FirstName,LastName,Address,EmailAddress,DOB,FatherName,MotherName,Occupation,PhoneNumber,Courses) values" +
                                "(@FirstName,@LastName,@Address,@EmailAddress,@DOB,@FatherName,@MotherName,@Occupation,@PhoneNumber,@Courses)";
                            cmd = new SqlCommand(query, connection);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FirstName", FirstName_Textbox.Text);
                            cmd.Parameters.AddWithValue("@LastName", SecondName_Textbox.Text);
                            cmd.Parameters.AddWithValue("@Address", Address_Textbox.Text);
                            cmd.Parameters.AddWithValue("@EmailAddress", Email_TextBox.Text);
                            cmd.Parameters.AddWithValue("@DOB", DOB_TextBox.Text);
                            cmd.Parameters.AddWithValue("@FatherName", FatherName_Textbox.Text);
                            cmd.Parameters.AddWithValue("@MotherName", MotherName_Textbox.Text);
                            cmd.Parameters.AddWithValue("@Occupation", Occupation_Textbox.Text);
                            cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber_TextBox.Text);
                            cmd.Parameters.AddWithValue("@Courses", Courses_TextBox.Text);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Contact Save Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    connection.Close();
                    ClearData();
                    GetStudentData();
                }
            }
            //when you press edit button this event will be in action as add ==Update
            else if (AddButton.Content.ToString() == "Update")
            {
                UpdateStudent(selectedStudentId);
                MainTabControl.SelectedIndex = 0;
                ClearData();
                AddButton.Content = "Add";
                GetStudentData();
                StudentAdderLabel.Content = "Student Adder";
            }

        }
        private void UpdateStudent(int StudentId)
        {

            try
            {
                mycon();
                string query = "UPDATE Student SET FirstName=@FirstName, LastName=@LastName, Address=@Address, EmailAddress=@EmailAddress, " +
            "DOB=@DOB, FatherName=@FatherName, MotherName=@MotherName, Occupation=@Occupation, " +
            "PhoneNumber=@PhoneNumber, Courses=@Courses WHERE Id=@StudentId";
                cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@FirstName", FirstName_Textbox.Text);
                cmd.Parameters.AddWithValue("@LastName", SecondName_Textbox.Text);
                cmd.Parameters.AddWithValue("@Address", Address_Textbox.Text);
                cmd.Parameters.AddWithValue("@EmailAddress", Email_TextBox.Text);
                cmd.Parameters.AddWithValue("@DOB", DateTime.Parse(DOB_TextBox.Text));
                cmd.Parameters.AddWithValue("@FatherName", FatherName_Textbox.Text);
                cmd.Parameters.AddWithValue("@MotherName", MotherName_Textbox.Text);
                cmd.Parameters.AddWithValue("@Occupation", Occupation_Textbox.Text);
                cmd.Parameters.AddWithValue("@PhoneNumber", int.Parse(PhoneNumber_TextBox.Text));
                cmd.Parameters.AddWithValue("@Courses", Courses_TextBox.Text);
                cmd.Parameters.AddWithValue("@StudentId", StudentId);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Data Update Sucessfully ", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally
            {

                connection.Close();
            }
        }
        #endregion
        #region Student Adder tap Validation
        private void Name_Validation(object sender, TextCompositionEventArgs e)
        {
            try
            {
                string pattern = @"^[a-zA-Z]$";
                if (!Regex.IsMatch(e.Text, pattern))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }


        }

        private void Address_Validation(object sender, TextCompositionEventArgs e)
        {
            string pattern = @"^[a-zA-Z0-9\s,.-]+$";
            if (!Regex.IsMatch(e.Text, pattern))
            {
                e.Handled = true;
            }
        }

        private void ValidateEmail()
        {
            if (!string.IsNullOrWhiteSpace(Email_TextBox.Text))
            {
                string email = Email_TextBox.Text;

                // Regular expression pattern to validate email addresses
                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";


                if (!Regex.IsMatch(email, emailPattern))
                {
                    MessageBox.Show("Invalid email address!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }

        }
        private void EmailTextBox_Validation(object sender, TextCompositionEventArgs e)
        {
            string validChars = @"[^a-zA-Z0-9@._-]"; // Allow letters, digits, @, ., _, -
            Regex regex = new Regex(validChars);
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ValidDOB()  //completed
        {
            Regex DobRegex = new Regex(@"^(0[1-9]|1[0-2])/(0[1-9]|[12][0-9]|3[01])/([0-9]{4})$");

            if (string.IsNullOrWhiteSpace(DOB_TextBox.Text))
            {
                DOB_TextBox.Text = PlaceholderText;

            }
            else if (!DobRegex.IsMatch(DOB_TextBox.Text))
            {
                MessageBox.Show("Invalid Date of Birth format. Please use MM/DD/YYYY.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DOB_TextBox.Text = PlaceholderText;


            }
        }
        private void DOBTextBox_Lostfocus(object sender, RoutedEventArgs e)  //completed
        {
            if (DOB_TextBox.Text != null || DOB_TextBox.Text == "")
            {
                ValidDOB();
            }
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Email_TextBox.Text != null || Email_TextBox.Text == "")
            {
                ValidateEmail();
            }
        }


        private void Course_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void PhoneNumber_Validation(object sender, TextCompositionEventArgs e)
        {

            TextBox textBox = (TextBox)sender;
            string text = textBox.Text + e.Text;

            string pattern = @"^[0-9]$";
            if (!Regex.IsMatch(e.Text, pattern) || text.Count(char.IsDigit) > 10)
            {
                e.Handled = true;
            }
        }

        private void Course_Validation(object sender, TextCompositionEventArgs e)
        {
            try
            {
                mycon();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }



        #endregion
        #endregion
        #region Course Tap

        private void ClearCourseData()
        {
            CourseId_Textbox.Text = string.Empty;
            CourseName_Textbox.Text = string.Empty;
            Credits_Textbox.Text = string.Empty;
            Instructors_TextBox.Text = string.Empty;
            Duration_TextBox.Text = string.Empty;

        }

        private void GetCourseData()
        {
            try
            {
                mycon();
                string query = "Select * from Courses";
                cmd = new SqlCommand(query, connection);
                DataTable coursedatatable = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(coursedatatable);

                if (coursedatatable.Rows.Count > 0)
                {
                    CourseDataViewerList.ItemsSource = coursedatatable.DefaultView;
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        private void CancelCourse_Button(object sender, RoutedEventArgs e)
        {
            ClearCourseData();
        }

        private void AddCourse_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(CourseId_Textbox.Text))
                {
                    MessageBox.Show("PLease Enter Course Id.");
                    return;
                }
                else if (string.IsNullOrEmpty(CourseName_Textbox.Text))
                {
                    MessageBox.Show("Please Enter Course Name");
                }
                else if (string.IsNullOrEmpty(Instructors_TextBox.Text))
                {
                    MessageBox.Show("Please Enter Instructor Name");
                }
                else
                {
                    if (MessageBox.Show("Do You Want To Add Course ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {

                        mycon();
                        string query = "Insert into Courses (Name , Credit , Instructor,Duration , CourseId) values(@Name,@Credit,@Instructor,@Duration,@CourseId)";
                        cmd = new SqlCommand(query, connection);
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("@Name", CourseName_Textbox.Text);
                        cmd.Parameters.AddWithValue("@Credit", Credits_Textbox.Text);
                        cmd.Parameters.AddWithValue("@Instructor", Instructors_TextBox.Text);
                        cmd.Parameters.AddWithValue("@Duration", Duration_TextBox.Text);
                        cmd.Parameters.AddWithValue("@CourseId", CourseId_Textbox.Text);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Course Added Successfully");
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
                ClearCourseData();
            }

        }
        #endregion

        private void CourseViewerList_SelectionChanges(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CourseEdit_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void CourseDelete_CLick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}