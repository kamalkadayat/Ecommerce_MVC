var dataTable;

$(document).ready(function () {
    // Load the table based on the current URL query string or default to "all"
    var url = window.location.search;
    var status = "all"; // Default status

    if (url.includes("inprocess")) {
        status = "inprocess";
    } else if (url.includes("completed")) {
        status = "completed";
    } else if (url.includes("pending")) {
        status = "pending";
    } else if (url.includes("approved")) {
        status = "approved";
    }

    // Load the initial table data
    loadDataTable(status);

    // Destroy any existing DataTable instance
    if ($.fn.DataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }

    // Event listener for the filter dropdown
    $('#filterStatus').on('change', function () {
        var selectedStatus = $(this).val();
        if ($.fn.DataTable.isDataTable('#tblData')) {
            $('#tblData').DataTable().destroy();
        }
        loadDataTable(selectedStatus); // Reload table with the selected status
    });
});

// Function to load DataTable based on status
function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/order/getall?status=' + status,
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
            { data: 'orderTotal' },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <div class="btn-group" role="group">
                            <a href="/admin/order/details?orderId=${data}" class="btn btn-primary">
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
}
