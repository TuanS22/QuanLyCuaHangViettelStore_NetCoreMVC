function login() {
    var objs = document.querySelectorAll('input');
    for (var i = 0; i < objs.length; i++) {
        var obj = objs[i];
        valiableInput(obj);
    }
}

function valiableInput(obj) {
    var valueObj = obj.value;
    var inputName = obj.getAttribute('name');
    var idError = 'error_' + inputName;
    var fiels_name = obj.getAttribute('field_name');
    var isError = false;

    if (obj.getAttribute('type') == 'email' && valueObj.indexOf('@') < 0) {
        document.getElementById(idError).innerHTML = 'Vui lòng nhập đúng định dạng ' + fiels_name;
        isError = true;
    }

    if (obj.getAttribute('type') == 'password' && (valueObj.length < 6 || valueObj.length > 12)) {
        document.getElementById(idError).innerHTML = 'Vui lòng nhập ' + fiels_name + ' 6 đến 12 ký tự';
        isError = true;
    }

    if (!isError) {
        document.getElementById(idError).innerHTML = '';
    }
}

function keyupInput(inputName) {
    var keyupObj = document.querySelector('input[name="' + inputName + '"]');
    valiableInput(keyupObj);
}