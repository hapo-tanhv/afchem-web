const hideBtn = document.getElementById('toggle');
const classA = document.getElementById('menubar');
const namebar = document.getElementById('namebar');
const body = document.getElementById('body');
hideBtn.addEventListener('click', function () {
    // Kiểm tra xem class 'a' đã được ẩn hay chưa
    var isHidden = classA.classList.contains('hidden');

    if (isHidden) {
        // Nếu class 'a' đã ẩn, loại bỏ class 'hidden' để hiển thị lại
        classA.classList.remove('hidden');
        namebar.style.marginLeft = '250px';
        body.style.marginLeft = '250px';
    } else {
        // Nếu class 'a' chưa ẩn, thêm class 'hidden' để ẩn đi
        classA.classList.add('hidden');
        namebar.style.marginLeft = 0;
        body.style.marginLeft = 0;
    }


});