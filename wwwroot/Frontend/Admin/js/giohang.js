$('.btn').on('click', function () {
    // Kiểm tra khi cách vào data-action
    var action = $(this).attr('data-action');

    // Lấy số lượng trong thẻ input
    var soluong = $(this).parent().find('input[name="soluong"]').val();

    // Lấy ra giá tiền trong thẻ input từ data-price
    var price = $(this).parent().find('input[name="soluong"]').attr('data-price');
     
    // Trừ
    if (action === 'tru') {
        // Khi nhấn trừ thì kiểm tra nếu số lượng lớn hơn 1 thì trừ -1 ngược lại gán = 1
        if (soluong > 1) {
            soluong = parseInt(soluong) - 1;
        }else {
            soluong = 1;
        }
    // Cộng
    }else {
        soluong = parseInt(soluong) + 1;
    }

    // Tính tổng 
    var totalPrice = soluong * price;

    // Cập nhập lại số lượng
    $(this).parent().find('input[name="soluong"]').val(soluong);

    // Cập nhập lại giá sản phẩm đúng định dạng qua hàm formatPrice
    $(this).parents('.products').find('.total_price').text(formatPrice(totalPrice));

    $(this).parents('.products').find('.total_price').attr('data-total-price', totalPrice);

    var totalAll = countTotalPrice();
    $('.total_all span').text(formatPrice(totalAll));
    
});

function countTotalPrice() {
    var totalAll = 0;
    // Duyệt qua từng phần tử total_price
    $('.total_price').each(function () {
        var totalPriceItem = $(this).attr('data-total-price');
        totalAll += parseInt(totalPriceItem);
    });
    return totalAll;
}

function formatPrice(price){
    price = price.toString();
    var count = price.length;
    
    var number = Math.ceil(count/3);
    var array = [];
    var du = count%3;

    for(var i = 0; i < number; i++){
        if(i == 0){
            if(du == 0){
                array[i] = price.substr(0, 3);
                du = du + 3;
            }else{
                array[i] = price.substr(0, du);
            }
        }else{
            if(du <= count){
                array[i] = price.substr(du, 3);
                du = du + 3;
            }
        }
    }
    var str = array.join('.');
    str += 'đ';
    return str;
}


// Xóa sản phẩm và cập nhật lại giá
$('.btn-remove').on('click', function () {
    $(this).parents('.products').remove();
    var totalAll = countTotalPrice();
    $('.total_all span').text(formatPrice(totalAll));
})