/**
 * Created by Lingo on 29/04/2017.
 */
function showExams() {
    var resultRef = firebase.database().ref('exams/'+getCookie("teacherID")+"/");
    resultRef.on('child_added', function(data) {
        $("<li id="+data.key+"><a href='showexam/?pid="+data.key+"&name="+data.val().examsName+"' class='open "+data.key+"'>" + data.val().examsName + "</a><a class='close' ><i class='fa fa-window-close-o' aria-hidden=true></i></a></li>").appendTo(".GroupList");
        $("a.close").click({param1: data.key}, deleteExam);
    });
}

function deleteExam($key) {
    if( PopupAlert("cvicenie ?") ) {
        var ref = firebase.database().ref('exams/'+getCookie("teacherID")+"/"+$key.data.param1);
        ref.remove(function(error) {
          $("#"+$key.data.param1).remove();
        });
        flashMsg("success","Cvicenie bolo odstranene");
    }
}

$( document ).ready(function() {
    showExams();
});