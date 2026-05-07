const image = document.getElementById('image');
const textFrame1 = document.getElementById('info');


function info() {
    const x = image.offsetLeft;
    const y = image.offsetTop;
    textFrame1.style.display = 'block';
    textFrame1.style.top = x+537 + 'px'; //tăng thì ảnh đi xuống
    textFrame1.style.left = y + 345 + 'px'; // giảm thì sang trái

   
}

textFrame1.addEventListener('click', function () {
    textFrame1.style.display = 'none';
});
