using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BusiBlocks.PersonLayer;
using BusiBlocks.Roles;
using System.Web.Services;
using BusiBlocks;

public partial class Admin_ManageRolesGroups : System.Web.UI.Page
{
    private const string ErrorDeletePersonTypeWithRoles = "Unable to delete. Disable all blocks for this group first";
    private const string ErrorDeletePersonTypeWithPersons = "Unable to delete. There are persons associated with this group";

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    [WebMethod]
    public static object GetPersonTypeRoles()
    {
        List<object> rows = PersonManager.GetAllPersonTypes(false).Cast<object>().ToList<object>();
        List<string> validRoles = new List<string>() 
            { 
                BusiBlocksConstants.Blocks.Communication.BlockName, 
                BusiBlocksConstants.Blocks.Documents.BlockName 
            };
        List<Role> roles = RoleManager.GetAllRoles().ToList<Role>();
        roles.Sort(new KeyComparer<Role>(x => (x.Name.Equals(BusiBlocksConstants.AdministratorsRole) ? "aaaa" : x.Name)));

        List<object> columns = new List<object>();
        foreach (Role role in roles)
        {
            if (validRoles.Contains(role.Name))
            {
                role.Name = RoleManager.CompressRoleName(role.Name);
                columns.Add(role);
            }
        }

        List<PersonTypeRole> personTypeRoles = PersonManager.GetAllPersonTypeRoles().ToList();
        var data = new List<PermissionMatrixDataItem>();
        foreach (PersonTypeRole ptr in personTypeRoles)
        {
            // Only add to the matrix if it is a role that is supported by this customer.
            if (validRoles.Contains(ptr.Role.Name))
                data.Add(new PermissionMatrixDataItem(ptr.Id, ptr.Role, ptr.PersonType));
        }

        return new PermissionMatrix(rows, columns, data);
    }

    [WebMethod]
    public static bool CreatePersonTypeRole(string columnId, string rowId)
    {
        string roleId = columnId;
        string personTypeId = rowId;

        try
        {
            var personTypeRole = new PersonTypeRole()
            {
                Role = RoleManager.GetRole(roleId),
                PersonType = PersonManager.GetPersonTypeById(personTypeId)
            };

            if (!PersonManager.IsPersonTypeInRole(personTypeId, roleId))
            {
                PersonManager.CreatePersonTypeRole(personTypeRole);
            }
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    [WebMethod]
    public static Dictionary<string, string> DeletePersonTypeRole(string columnId, string rowId)
    {
        string roleId = columnId;
        string personTypeId = rowId;

        bool exists = true;
        bool deleted = false;

        Dictionary<string, string> returnValue = new Dictionary<string, string>();

        try
        {
            PersonType personType = PersonManager.GetPersonTypeById(personTypeId);

            if (personType == null)
            {
                exists = false;
            }
            else
            {
                IList<PersonTypeRole> personTypeRoles = PersonManager.GetPersonTypeRoleByPerson(personType);

                if (personTypeRoles.Count == 0)
                {
                    exists = false;
                }
                else
                {
                    foreach (PersonTypeRole personTypeRole in personTypeRoles)
                    {
                        if (personTypeRole.Role.Id == roleId)
                        {
                            if (!(personTypeRole.Role.Name.Equals(BusiBlocksConstants.AdministratorsRole) && personTypeRole.PersonType.Name.Equals(BusiBlocksConstants.AdministratorsGroup)))
                            {
                                PersonManager.DeletePersonTypeRole(personTypeRole);
                                deleted = true;
                                exists = false;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            returnValue.Add("error", "true");
            returnValue.Add("ex", ex.ToString().ToLower());
        }

        returnValue.Add("exists", exists.ToString(CultureInfo.InvariantCulture).ToLower());
        returnValue.Add("deleted", deleted.ToString(CultureInfo.InvariantCulture).ToLower());

        return returnValue;
    }

    protected void DeleteGroup_Click(object sender, EventArgs e)
    {
        string groupId = popDeleteGroup.ReferrerId;

        try
        {
            PersonType personType = PersonManager.GetPersonTypeById(groupId);
            IList<PersonTypeRole> roles = PersonManager.GetPersonTypeRoleByPerson(personType);
            if (roles.Count > 0)
            {
                ((IFeedback)Master).SetError(GetType(), ErrorDeletePersonTypeWithRoles);
            }
            else
            {
                // Are there any people assigned to this person type?
                IList<Person> peopleInGroup = PersonManager.GetAllPersonsByPersonType(personType);

                if (peopleInGroup.Count > 0)
                {
                    ((IFeedback)Master).SetError(GetType(), ErrorDeletePersonTypeWithPersons);
                }
                else
                {
                    PersonManager.DeletePersonType(personType.Id);

                    ((IFeedback)Master).ShowFeedback(
                        BusiBlocksConstants.Blocks.Administration.LongName,
                        personType.GetType().Name,
                        Feedback.Actions.Deleted,
                        personType.Name
                    );
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
            ((IFeedback)Master).SetException(GetType(), ex);
        }
    }
}