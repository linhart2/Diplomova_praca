/**
 * Created by Lingo on 29/04/2017.
 */
function showExams() {
    var resultRef = firebase.database().ref('exams/'+getCookie("teacherID")+"/");
    resultRef.on('child_added', function(data) {
        $("<li id="+data.key+"><a class='open'>" + data.val().examsName + "</a><a class='close' ><i class='fa fa-window-close-o' aria-hidden=true></i></a></li>").appendTo(".GroupList");
        $("a.close").click({param1: data.key}, deleteExam);
        $("a.open").click({param1: data.key}, openExam);
    });
}
function openExam($examName) {
    $(".GroupList").empty();
    var resultRef = firebase.database().ref('exams/'+getCookie("teacherID")+"/"+$examName.data.param1+"/");
    resultRef.once('value', function(data) {
        data.forEach(function (snapshot) {
            console.log(snapshot.key+" "+snapshot.val());
        })
        $("<li><a>" + data.key + "</a><a class='close' onclick=return PopupAlert('studenta')><i class='fa fa-window-close-o' aria-hidden=true></i></a><a class='edit' ><i class='fa fa-pencil-square-o' aria-hidden=true></i></a></li>").appendTo(".GroupList");
    });

}

function deleteExam($key) {
    if( PopupAlert("cvicenie ?") ) {
        var ref = firebase.database().ref('exams/'+getCookie("teacherID")+"/"+$key.data.param1);
        ref.remove(function(error) {
          alert(error ? "Uh oh!" : "Success!");
          $("#"+$key.data.param1).remove();
        });
    }
}

function editExam() {
    console.log("ssss");
}

$( document ).ready(function() {
    showExams();
});