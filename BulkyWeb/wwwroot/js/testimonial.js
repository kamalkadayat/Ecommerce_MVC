﻿var dataTable;

$(document).ready(function () {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/testimonial/getall',
            dataSrc: 'data'
        },
        "columns": [
            { data: 'clientName' },
            { data: 'message' },
          
            {
                data: 'clientImage',
                "render": function (data) {
                    return `<img src="${data}" alt="Client Image" width="100" height="auto" />`;
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/testimonial/upsert?id=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i>Edit</a >
                        <a onClick="Delete('/admin/testimonial/delete/${data}')" class="btn btn-danger mx-2"><i class="bi bi-trash"></i>Delete</a>                            
                        </div > `
                }
            }

        ],
        "columnDefs": [
            { "width": "20%", "targets": 0 }, 
            { "width": "45%", "targets": 1 }, 
            { "width": "15%", "targets": 2 },
            { "width": "20%", "targets": 3 } 
        ]
    });
});

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}