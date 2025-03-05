using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using System.Collections.Specialized;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using OfficeOpenXml;
using System.IO;

namespace EntityAccessManagement
{
    public partial class MyPluginControl : MultipleConnectionsPluginControlBase
    {
        private Settings mySettings;

        public MyPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();
                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }

            ExecuteMethod(GetEntities);
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }
        public void GetEntities()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading entities...",
                Work = (worker, args) =>
                {
                    RetrieveAllEntitiesRequest metaDataRequest = new RetrieveAllEntitiesRequest
                    {
                        EntityFilters = EntityFilters.Entity
                    };
                    // Execute the request.
                    RetrieveAllEntitiesResponse metaDataResponse = (RetrieveAllEntitiesResponse)Service.Execute(metaDataRequest);
                    var entities = metaDataResponse.EntityMetadata;

                    args.Result = entities;
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityMetadata[];
                    foreach (var entity in result)
                    {
                        cbEntities.Items.Add(entity.LogicalName);
                    }
                    cbEntities.SelectedIndex = 0;
                }
            });
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
            }
        }
        protected override void ConnectionDetailsUpdated(NotifyCollectionChangedEventArgs e)
        {

        }

        private void tsbReloadentity_Click(object sender, EventArgs e)
        {
            ExecuteMethod(GetEntities);
        }

        public EntityPrivileges EntityPrivileges;
        private void tsbLoadpermissions_Click(object sender, EventArgs e)
        {
            if (cbEntities.Items.Count <= 0 || cbEntities.SelectedIndex < 0)
            {
                MessageBox.Show(this, "Please select an entity", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            EntityPrivileges = new EntityPrivileges
            {
                EntityName = cbEntities.SelectedItem.ToString()
            };

            Loadpermissions();
        }

        private void Loadpermissions()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = $"Loading permissions...",
                Work = (worker, args) =>
                {
                    EntityCollection roles = CRMHelper.GetAllRoles(Service);
                    List<RolePrivileges> list_RolePrivileges = new List<RolePrivileges>();
                    foreach (var item in roles.Entities)
                    {
                        RolePrivileges privileges = new RolePrivileges
                        {
                            RoleId = item.Id,
                            RoleName = item["name"].ToString(),
                            BusinessUnitId = (item["businessunitid"] as EntityReference).Id,
                            CreateImag = EntityPrivileges.GetPrivilegeLevel(0),
                            ReadImag = EntityPrivileges.GetPrivilegeLevel(0),
                            WriteImag = EntityPrivileges.GetPrivilegeLevel(0),
                            DeleteImag = EntityPrivileges.GetPrivilegeLevel(0),
                            AppendImag = EntityPrivileges.GetPrivilegeLevel(0),
                            AppendToImag = EntityPrivileges.GetPrivilegeLevel(0),
                            AssignImag = EntityPrivileges.GetPrivilegeLevel(0),
                            ShareImag = EntityPrivileges.GetPrivilegeLevel(0)
                        };
                        list_RolePrivileges.Add(privileges);
                    }
                    EntityPrivileges.RolePrivileges = list_RolePrivileges;

                    foreach (PrivilegeType privilegeType in Enum.GetValues(typeof(PrivilegeType)))
                    {
                        Entity Privilege = CRMHelper.GetPrivilege(Service, $"{privilegeType}{EntityPrivileges.EntityName}");
                        if (Privilege == null)
                        {
                            continue;
                        }
                        EntityPrivileges.SetPrivilegeId(privilegeType, Privilege);
                        EntityCollection entityCollection = CRMHelper.GetRoleprivileges(Service, Privilege.Id.ToString());
                        foreach (var item in entityCollection.Entities)
                        {
                            EntityPrivileges.SetRolePrivileges(privilegeType, item);
                        }
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    dataGridView1.DataSource = EntityPrivileges.RolePrivileges.OrderBy(t => t.RoleName).ToList();

                    dataGridView1.Columns["RoleId"].Visible = false;
                    dataGridView1.Columns["BusinessUnitId"].Visible = false;
                    dataGridView1.Columns["CreatePrivilegeId"].Visible = false;
                    dataGridView1.Columns["CreateDepth"].Visible = false;
                    dataGridView1.Columns["ReadPrivilegeId"].Visible = false;
                    dataGridView1.Columns["ReadDepth"].Visible = false;
                    dataGridView1.Columns["WritePrivilegeId"].Visible = false;
                    dataGridView1.Columns["WriteDepth"].Visible = false;
                    dataGridView1.Columns["DeletePrivilegeId"].Visible = false;
                    dataGridView1.Columns["DeleteDepth"].Visible = false;
                    dataGridView1.Columns["AppendPrivilegeId"].Visible = false;
                    dataGridView1.Columns["AppendDepth"].Visible = false;
                    dataGridView1.Columns["AppendToPrivilegeId"].Visible = false;
                    dataGridView1.Columns["AppendToDepth"].Visible = false;
                    dataGridView1.Columns["AssignPrivilegeId"].Visible = false;
                    dataGridView1.Columns["AssignDepth"].Visible = false;
                    dataGridView1.Columns["SharePrivilegeId"].Visible = false;
                    dataGridView1.Columns["ShareDepth"].Visible = false;
                }
            });
        }

        private readonly List<ProcessList> processLists = new List<ProcessList>();
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var CurrentCell = this.dataGridView1.CurrentCell;
                var prvName = CurrentCell.OwningColumn.Name;
                if (CurrentCell.ValueType.Name == "Image")
                {
                    ProcessList process = new ProcessList();

                    Image image = (Image)CurrentCell.Value;
                    int tag = (int)image.Tag;
                    if (tag == 0)
                    {
                        process.Depth = 1;

                        this.dataGridView1.Rows[CurrentCell.RowIndex].Cells[CurrentCell.ColumnIndex].Value = EntityPrivileges.GetPrivilegeLevel(1);
                    }
                    else if (tag == 8)
                    {
                        process.Depth = 0;
                        this.dataGridView1.Rows[CurrentCell.RowIndex].Cells[CurrentCell.ColumnIndex].Value = EntityPrivileges.GetPrivilegeLevel(0);
                    }
                    else
                    {
                        process.Depth = tag * 2;
                        this.dataGridView1.Rows[CurrentCell.RowIndex].Cells[CurrentCell.ColumnIndex].Value = EntityPrivileges.GetPrivilegeLevel(tag * 2);
                    }

                    process.RoleId = (Guid)CurrentCell.OwningRow.Cells["RoleId"].Value;
                    process.BusinessUnitId = (Guid)CurrentCell.OwningRow.Cells["BusinessUnitId"].Value;
                    switch (prvName)
                    {
                        case "CreateImag":
                            process.PrivilegeId = (Guid)CurrentCell.OwningRow.Cells["CreatePrivilegeId"].Value;
                            process.Old_Depth = (int)CurrentCell.OwningRow.Cells["CreateDepth"].Value;
                            break;
                        case "ReadImag":
                            process.PrivilegeId = (Guid)CurrentCell.OwningRow.Cells["ReadPrivilegeId"].Value;
                            process.Old_Depth = (int)CurrentCell.OwningRow.Cells["ReadDepth"].Value;
                            break;
                        case "WriteImag":
                            process.PrivilegeId = (Guid)CurrentCell.OwningRow.Cells["WritePrivilegeId"].Value;
                            process.Old_Depth = (int)CurrentCell.OwningRow.Cells["WriteDepth"].Value;
                            break;
                        case "DeleteImag":
                            process.PrivilegeId = (Guid)CurrentCell.OwningRow.Cells["DeletePrivilegeId"].Value;
                            process.Old_Depth = (int)CurrentCell.OwningRow.Cells["DeleteDepth"].Value;
                            break;
                        case "AppendImag":
                            process.PrivilegeId = (Guid)CurrentCell.OwningRow.Cells["AppendPrivilegeId"].Value;
                            process.Old_Depth = (int)CurrentCell.OwningRow.Cells["AppendDepth"].Value;
                            break;
                        case "AppendToImag":
                            process.PrivilegeId = (Guid)CurrentCell.OwningRow.Cells["AppendToPrivilegeId"].Value;
                            process.Old_Depth = (int)CurrentCell.OwningRow.Cells["AppendToDepth"].Value;
                            break;
                        case "AssignImag":
                            process.PrivilegeId = (Guid)CurrentCell.OwningRow.Cells["AssignPrivilegeId"].Value;
                            process.Old_Depth = (int)CurrentCell.OwningRow.Cells["AssignDepth"].Value;
                            break;
                        case "ShareImag":
                            process.PrivilegeId = (Guid)CurrentCell.OwningRow.Cells["SharePrivilegeId"].Value;
                            process.Old_Depth = (int)CurrentCell.OwningRow.Cells["ShareDepth"].Value;
                            break;
                    }
                    var obj = processLists.Where(t => t.RoleId == process.RoleId && t.PrivilegeId == process.PrivilegeId).FirstOrDefault();
                    if (obj != null)
                    {
                        processLists.Remove(obj);
                    }

                    processLists.Add(process);
                }
            }
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            var temp_processLists = processLists.Where(t => t.Depth != t.Old_Depth).ToList();

            if (processLists.Count <= 0 || temp_processLists.Count <= 0)
            {
                MessageBox.Show(this, "Did not change any permissions", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            WorkAsync(new WorkAsyncInfo
            {
                Message = $"Save permissions...",
                Work = (worker, args) =>
                {
                    foreach (var item in temp_processLists)
                    {
                        LogInfo($"RoleId:{item.RoleId},PrivilegeId:{item.PrivilegeId},Depth:{item.Depth},Old_Depth:{item.Old_Depth}");
                        CRMHelper.UpdatePrivilegesRole(Service, item);
                    }

                    processLists.Clear();
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    MessageBox.Show(this, "Successfully modified", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Loadpermissions();
                }
            });

        }

        public void ExportRolePrivilegesToExcel()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            // Create a new Excel package
            using (ExcelPackage package = new ExcelPackage())
            {
                // Create a new worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Role Privileges");

                // Set the column headers
                worksheet.Cells[1, 1].Value = "Role Name";
                worksheet.Cells[1, 2].Value = "Entity Name";
                worksheet.Cells[1, 3].Value = "Create Privilege";
                worksheet.Cells[1, 4].Value = "Read Privilege";
                worksheet.Cells[1, 5].Value = "Write Privilege";
                worksheet.Cells[1, 6].Value = "Delete Privilege";
                worksheet.Cells[1, 7].Value = "Append Privilege";
                worksheet.Cells[1, 8].Value = "Append To Privilege";
                worksheet.Cells[1, 9].Value = "Assign Privilege";
                worksheet.Cells[1, 10].Value = "Share Privilege";

                // Set the data rows
                int row = 2;
                foreach (var rolePrivilege in EntityPrivileges.RolePrivileges)
                {
                    worksheet.Cells[row, 1].Value = rolePrivilege.RoleName;
                    worksheet.Cells[row, 2].Value = EntityPrivileges.EntityName;
                    worksheet.Cells[row, 3].Value = GetPermissionlevel(rolePrivilege.CreateDepth);
                    worksheet.Cells[row, 4].Value = GetPermissionlevel(rolePrivilege.ReadDepth);
                    worksheet.Cells[row, 5].Value = GetPermissionlevel(rolePrivilege.WriteDepth);
                    worksheet.Cells[row, 6].Value = GetPermissionlevel(rolePrivilege.DeleteDepth);
                    worksheet.Cells[row, 7].Value = GetPermissionlevel(rolePrivilege.AppendDepth);
                    worksheet.Cells[row, 8].Value = GetPermissionlevel(rolePrivilege.AppendToDepth);
                    worksheet.Cells[row, 9].Value = GetPermissionlevel(rolePrivilege.AssignDepth);
                    worksheet.Cells[row, 10].Value = GetPermissionlevel(rolePrivilege.ShareDepth);

                    row++;
                }

                // Auto fit the columns
                worksheet.Cells.AutoFitColumns();

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                string filePath = Path.Combine(desktopPath, EntityPrivileges.EntityName + "RolePrivileges.xlsx");
                FileInfo file = new FileInfo(filePath);
                package.SaveAs(file);
                MessageBox.Show(this, "Export to desktop successful!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string GetPermissionlevel(int Depth)
        {
            switch (Depth)
            {
                case 0:return "None";
                case 1:return "User";
                case 2:return "Business Unit";
                case 4:return "Parental";
                case 8:return "Organization";
                default:
                    return "";
            }
        }

        private void tabExcel_Click(object sender, EventArgs e)
        {
            if(EntityPrivileges.RolePrivileges==null|| EntityPrivileges.RolePrivileges.Count == 0)
            {
                MessageBox.Show(this, "RolePrivileges Not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ExportRolePrivilegesToExcel();
        }

        private void tsbCopyFromEntity_Click(object sender, EventArgs e)
        {

        }
    }
}