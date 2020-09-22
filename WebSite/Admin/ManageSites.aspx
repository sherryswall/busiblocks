<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageSites.aspx.cs"
    Inherits="Admin_ManageSites" Title="Manage Sites" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <script language="javascript" type="text/javascript">


        function copyAddress(sameAsPostal) {

            var firstName = document.getElementById('txtPostalAddress1').value;
            //            var lastName = document.getElementById('billing_last_name').value;
            //            var streetAddress = document.getElementById('street_address-5').value;
            //            var city = document.getElementById('city-5').value;
            //            var stateProvince = document.getElementById('state_province_id-5').value;
            //            var postalCode = document.getElementById('postal_code-5').value;
            //            var country = document.getElementById('country_id-5').value;

            var copy_firstName = document.getElementById('txtPhysicalAddress1');
            //            var copy_lastName = document.getElementById('last_name');
            //            var copy_streetAddress = document.getElementById('street_address-Primary');
            //            var copy_city = document.getElementById('city-Primary');
            //            var copy_stateProvince = document.getElementById('state_province-Primary');
            //            var copy_postalCode = document.getElementById('postal_code-Primary');
            //            var copy_country = document.getElementById('country-Primary');

            if (sameAsPostal.checked) {
                if (copy_firstName) copy_firstName.value = firstName;
                //                if (copy_lastName) copy_lastName.value = lastName;
                //                if (copy_streetAddress) copy_streetAddress.value = streetAddress;
                //                if (copy_city) copy_city.value = city;
                //                if (copy_stateProvince) copy_stateProvince.value = stateProvince;
                //                if (copy_postalCode) copy_postalCode.value = postalCode;
                //                if (copy_country) copy_country.value = country;
            } else {
                if (copy_firstName) copy_firstName.value = "";
                //                if (copy_lastName) copy_lastName.value = "";
                //                if (copy_streetAddress) copy_streetAddress.value = "";
                //                if (copy_city) copy_city.value = "";
                //                if (copy_stateProvince) copy_stateProvince.value = "";
                //                if (copy_postalCode) copy_postalCode.value = "";
                //                if (copy_country) copy_country.value = "";
            }
        }
    </script>
    <h1 class="sectionhead" id="sites">
        <asp:Label ID="lblPageTitle" runat="server" /></h1>
    <asp:Label ID="lblSiteId" runat="server" Visible="false" />
    <table class="formTable">
        <tr class="primary">
            <td>
                Site Name
            </td>
            <td>
                <asp:TextBox ID="txtSiteName" runat="server" />
                <label style="color:Red">*</label>
                <asp:RequiredFieldValidator ID="rfvSiteName" runat="server" ControlToValidate="txtSiteName"
                    ErrorMessage="a site name is required" ForeColor="Red" ValidationGroup="valSiteDetails"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="validSiteName" runat="server" ControlToValidate="txtSiteName"
                    ErrorMessage="valid site name required"  ValidationGroup="valSiteDetails" ValidationExpression="^[a-zA-Z0-9\s\-]{0,99}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Phone Number
            </td>
            <td>
                <asp:TextBox ID="txtPhoneNumber" runat="server" />
                <asp:RegularExpressionValidator ID="validPhoneNumber" runat="server" ControlToValidate="txtPhoneNumber"
                    ValidationGroup="valSiteDetails" ErrorMessage="valid phone number required"
                    ValidationExpression="^\+?[0-9\/.()-]{9,}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Alternate Phone Number
            </td>
            <td>
                <asp:TextBox ID="txtAltPhoneNumber" runat="server" />
                <asp:RegularExpressionValidator ID="validAltPhoneNumber" runat="server" ControlToValidate="txtAltPhoneNumber"
                    ValidationGroup="valSiteDetails" ErrorMessage="valid phone number required"
                    ValidationExpression="^\+?[0-9\/.()-]{9,}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Email
            </td>
            <td>
                <asp:TextBox ID="txtEmail" runat="server" />
                <asp:RegularExpressionValidator ID="validEmail" runat="server" ControlToValidate="txtEmail"
                    ValidationGroup="valSiteDetails" ErrorMessage="valid email address required"
                    ValidationExpression="^([a-zA-Z0-9\.-]+@[a-zA-Z0-9\.-]+[\.][a-zA-Z0-9\.-]+){1,70}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Postal Address 1
            </td>
            <td>
                <asp:TextBox ID="txtPostalAddress1" runat="server" />
                <asp:RegularExpressionValidator ID="validPostalNumber" runat="server" ControlToValidate="txtPostalAddress1"
                     ValidationGroup="valSiteDetails" ErrorMessage="valid number required"
                    ValidationExpression="^[a-zA-Z0-9\s\-]+$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Postal Address 2
            </td>
            <td>
                <asp:TextBox ID="txtPostalAddress2" runat="server" />
                <asp:RegularExpressionValidator ID="validPostalStreet" runat="server" ControlToValidate="txtPostalAddress2"
                    ValidationGroup="valSiteDetails" ErrorMessage="valid street name required"
                    ValidationExpression="^[a-zA-Z0-9\s\-]+$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Postal Suburb
            </td>
            <td>
                <asp:TextBox ID="txtPostalSuburb" runat="server" />
                <asp:RegularExpressionValidator ID="validPostalSuburb" runat="server" ControlToValidate="txtPostalSuburb"
                     ValidationGroup="valSiteDetails" ErrorMessage="valid suburb name required"
                    ValidationExpression="^[a-zA-Z\s]+$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Postal Postcode
            </td>
            <td>
                <asp:TextBox ID="txtPostalPostcode" runat="server" />
                <asp:RegularExpressionValidator ID="validPostalPostcode" runat="server" ControlToValidate="txtPostalPostcode"
                     ValidationGroup="valSiteDetails" ErrorMessage="valid postcode required"
                    ValidationExpression="^[0-9]{4}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Postal State
            </td>
            <td>
                <asp:TextBox ID="txtPostalState" runat="server" />
            </td>
        </tr>
        <%--<tr>
            <td>
                Physical Address same as Postal
            </td>
            <td>
                <asp:CheckBox ID="chkSameAs" runat="server" onclientclick="copyAddress(this);" /> 
            </td>
        </tr>--%>
        <tr>
            <td>
                Physical Address 1
            </td>
            <td>
                <asp:TextBox ID="txtPhysicalAddress1" runat="server" />
                <asp:RegularExpressionValidator ID="validNumber" runat="server" ControlToValidate="txtPhysicalAddress1"
                    ValidationGroup="valSiteDetails" ErrorMessage="valid number required"
                    ValidationExpression="^[a-zA-Z0-9\s\-]+$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Physical Address 2
            </td>
            <td>
                <asp:TextBox ID="txtPhysicalAddress2" runat="server" />
                <asp:RegularExpressionValidator ID="validStreetName" runat="server" ControlToValidate="txtPhysicalAddress2"
                     ValidationGroup="valSiteDetails" ErrorMessage="valid street name required"
                    ValidationExpression="^[a-zA-Z0-9\s\-]+$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Physical Suburb
            </td>
            <td>
                <asp:TextBox ID="txtPhysicalSuburb" runat="server" />
                <asp:RegularExpressionValidator ID="validSuburb" runat="server" ControlToValidate="txtPhysicalSuburb"
                    ValidationGroup="valSiteDetails" ErrorMessage="valid suburb name required"
                    ValidationExpression="^[a-zA-Z\s]+$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Physical Postcode
            </td>
            <td>
                <asp:TextBox ID="txtPhysicalPostcode" runat="server" />
                <asp:RegularExpressionValidator ID="validPostcode" runat="server" ControlToValidate="txtPhysicalPostcode"
                    ValidationGroup="valSiteDetails" ErrorMessage="valid postcode required"
                    ValidationExpression="^[0-9]{4}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td>
                Physical State
            </td>
            <td>
                <asp:TextBox ID="txtPhysicalState" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                Region
            </td>
            <td>
                <asp:DropDownList ID="cmbRegions" runat="server" />
            </td>
        </tr>
    </table>
    <asp:Button ID="btnUpdate" runat="server" Text="Save" CssClass="btn" OnClick="btnUpdate_Click"
        ValidationGroup="valSiteDetails" />
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
</asp:Content>
