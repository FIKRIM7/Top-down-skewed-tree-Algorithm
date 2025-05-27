// JSON Polyfill for older browsers
if (!window.JSON) {
    window.JSON = {
        stringify: function (obj) {
            var t = typeof obj;
            if (t != "object" || obj === null) {
                if (t == "string") return '"' + obj.replace(/"/g, '\\"') + '"';
                return String(obj);
            }
            var n, v, json = [], arr = (obj && obj.constructor == Array);
            for (n in obj) {
                v = obj[n];
                t = typeof v;
                if (t == "string") v = '"' + v.replace(/"/g, '\\"') + '"';
                else if (t == "object" && v !== null) v = this.stringify(v);
                json.push((arr ? "" : '"' + n + '":') + String(v));
            }
            return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}");
        }
    };
}

function addEvent(element, event, handler) {
    if (element.addEventListener) {
        element.addEventListener(event, handler, false);
    } else if (element.attachEvent) {
        element.attachEvent('on' + event, handler);
    }
}

function ready(callback) {
    if (document.addEventListener) {
        addEvent(document, 'DOMContentLoaded', callback);
    } else {
        addEvent(window, 'load', callback);
    }
}

function getElementsByClassName(className, tag, parent) {
    parent = parent || document;
    var elements = parent.getElementsByTagName(tag || '*');
    var matches = [];
    for (var i = 0; i < elements.length; i++) {
        if ((' ' + elements[i].className + ' ').indexOf(' ' + className + ' ') > -1) {
            matches.push(elements[i]);
        }
    }
    return matches;
}

ready(function () {
    var form = document.getElementById('quizForm');
    var submitButton = document.getElementById('quizSubmit');
    var questions = getElementsByClassName('quiz-question', 'div', form);

    addEvent(form, 'submit', function (e) {
        if (e.preventDefault) {
            e.preventDefault();
        } else {
            e.returnValue = false;
        }

        var score = 0;
        var results = [];

        for (var i = 0; i < questions.length; i++) {
            var question = questions[i];
            var inputs = question.getElementsByTagName('input');
            var selectedOption = null;
            for (var j = 0; j < inputs.length; j++) {
                if (inputs[j].checked) {
                    selectedOption = inputs[j];
                    break;
                }
            }
            var isCorrect = selectedOption && (' ' + selectedOption.parentNode.className + ' ').indexOf(' correct ') > -1;

            if (isCorrect) {
                score++;
            }

            var h3 = question.getElementsByTagName('h3')[0];
            var selectedAnswer = selectedOption ? (selectedOption.nextSibling.innerText || selectedOption.nextSibling.textContent) : 'No answer';

            results.push({
                question: h3.innerText || h3.textContent,
                selectedAnswer: selectedAnswer,
                isCorrect: isCorrect
            });
        }

        alert('Вы ответили правильно на ' + score + ' из ' + questions.length + ' вопросов!');

        var downloadButton = document.createElement('button');
        downloadButton.innerText = 'Показать результаты';
        downloadButton.className = 'quiz-submit download-button';
        addEvent(downloadButton, 'click', function () {
            downloadResults(results);
        });

        if (!getElementsByClassName('download-button', '*', form).length) {
            form.appendChild(downloadButton);
        }
    });

    function downloadResults(results) {
        var dataStr = JSON.stringify(results, null, 2);
        var textarea = document.createElement('textarea');
        textarea.value = dataStr;
        textarea.style.width = '100%';
        textarea.style.height = '200px';
        textarea.style.marginTop = '10px';
        document.getElementById('quizForm').appendChild(textarea);
        textarea.select();
        //alert('JSON data is displayed in the textarea below. Please copy it and save to a file named "quiz-results.json".');
    }
});