var dataTable;

$(document).ready(function () {
    if ($.fn.DataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }

    $('#tblData').DataTable({
        "ajax": {
            url: '/admin/order/getall',
            dataSrc: 'data' 
        },
        "columns": [
            { data: 'id' },
            { data: 'name' },
            { data: 'phoneNumber' },
            {
                data: null,
                render: function (data, type, row) {
                    return data.applicationUser ? data.applicationUser.email : 'N/A';
                }
            },
            { data: 'orderStatus' },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <div class="btn-group" role="group">
                            <a href="/admin/order/orderId?id=${data}" class="btn btn-primary">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>`;
                }
            }
        ],
        "order": [[0, "asc"]],
        "paging": true,
        "searching": true,
        "info": true
    });
});
