/**
 * Created by Lingo on 29/04/2017.
 */
function showExams() {
    $("h1.examname").empty().append(getURLParameter('name'));
    var resultRef = firebase.database().ref('exams/'+getCookie("teacherID")+"/"+getURLParameter('pid')+"/");
    resultRef.on('child_added', function(data) {
        if(data.key !== 'examsName'){
            $("<li id="+data.key+"><a href='openexam/?pid="+getURLParameter('pid')+"&name="+data.key+"' class='open'>" + data.key + "</a><a href='editexam/?pid="+getURLParameter('pid')+"&name="+data.key+"' class='edit'><i class='fa fa-pencil-square-o' aria-hidden='true'></i></a><a class='close "+data.key+"' ><i class='fa fa-window-close-o' aria-hidden=true></i></a></li>").appendTo(".GroupList");
            $("a."+data.key).click({param1: data.key}, deleteExam);
        }
    });
}

function deleteExam($key) {
    if( PopupAlert("priklad ?") ) {
        var ref = firebase.database().ref('exams/'+getCookie("teacherID")+"/"+getURLParameter('pid'));
        ref.once('value',function (data) {
           if(data.numChildren() <= 2){
               var ref = firebase.database().ref('exams/'+getCookie("teacherID")+"/"+getURLParameter('pid'));
               ref.remove(function(error) {
                   $("#"+$key.data.param1).remove();
               });
               window.location.replace("../../www/application/showexams/");
               flashMsg("success","Priklad bol odstraneny");
           }
           else{
               var ref = firebase.database().ref('exams/'+getCookie("teacherID")+"/"+getURLParameter('pid')+"/"+$key.data.param1);
               ref.remove(function(error) {
                   $("#"+$key.data.param1).remove();
               });
               flashMsg("success","Priklad bol odstraneny");
           }
        });
    }
}

$( document ).ready(function() {
    showExams();
});