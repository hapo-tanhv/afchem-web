
function changePassword() {

    var username = document.getElementById('UserName').value;
    var oldPassword = document.getElementById('OldPassword').value;
    var newPassword = document.getElementById('NewPassword').value;
    var confirmPassword = document.getElementById('ConfirmPassword').value;

    var param = {
        UserName: username,
        OldPassword: oldPassword,
        NewPassword: newPassword,
        ConfirmPassword: confirmPassword
    };

    $.ajax({
        type: "POST",
        url: '/Home/ChangePassword',
        data: JSON.stringify(param),
        contentType: "application/json; charset=utf-8",
        timeout: 50000,
        dataType: "json",
        success: function (data) {
            if (data.Status) {
                alert(data.Message);
                // Suceess
            }
            else {
                alert(data.Message);
                // Error
            }            
        },
        error: function () {
            alert("Something went wrong");
            //Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });

}