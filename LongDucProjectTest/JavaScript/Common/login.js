
function login() {

    var username = document.getElementById('UserName').value;
    var password = document.getElementById('Password').value;

    var dataLogin = {
        UserName: username,
        Password: password
    };

    $.ajax({
        type: "POST",
        url: '/Home/Login',
        data: JSON.stringify(dataLogin),
        contentType: "application/json; charset=utf-8",
        timeout: 5000,
        dataType: "json",
        success: function (data) {
            if(!data.Result)
            alert("success");
        },
        error: function () {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });

}