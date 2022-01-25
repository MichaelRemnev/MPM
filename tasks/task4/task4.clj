(ns task4)

(defn constant [num]   ;; Порождение константы
  (list ::const, num))

(defn constant? [expr] ;; Проверка, является ли выражение константой
  (= (first expr), ::const))

(defn constant-value [const]  ;; Получение значения константы
  (second const))


(defn variable [name]  ;; Порождение переменной
  {:pre [(keyword? name)]}
  (list ::var name))

(defn variable? [expr] ;; Проверка, является ли выражение переменной
  (= (first expr), ::var))

(defn variable-name [v] ;; Получение значения для переменной
  (second v))

(defn same-variables? [v1, v2]  ;; Сравнение переменных
  (and (variable? v1), (variable? v2), (= (variable-name v1), (variable-name v2))))


(defn && [expr & rest]  ;; Порожднеие конъюнкции
  (if (empty? rest)
    expr
    (cons ::conj, (cons expr, rest))))

(defn &&? [expr]  ;; Проверка, является ли выражение конъюнкцией
  (= ::conj, (first expr)))

(defn || [expr & rest]  ;; Порожднеие дизъюнкции
  (if (empty? rest)
    expr
    (cons ::disj, (cons expr, rest))))

(defn ||? [expr]  ;; Проверка, является ли выражение дизъюнкцией
  (= ::disj, (first expr)))

(defn --> [from to]  ;; Порожднеие импликации
  (cons ::impl, (list from, to)))

(defn ->? [expr]  ;; Проверка, является ли выражение кмпликацией
  (= ::impl, (first expr)))

(defn no? [expr]  ;; Проверка, является ли выражение отрицанием
  (= ::neg, (first expr)))

(defn no [expr]  ;; Порожднеие отрицания
  (if (no? expr)
    (second expr)
    (list ::neg, expr)))

(defn args [expr] ;; Список аргументов выражения
  (rest expr))

(defn arg [expr]  ;; Первый аргумент
  (second expr))

(defn print-dnf [expr]
  (let [print-rules (list
                     [(fn [expr] (or (constant? expr), (variable? expr))) (fn [expr] (print (arg expr)))]
                     [->? (fn [expr] (do (print "(") (print-dnf (first (args expr))) (print " -> ") (print-dnf (second (args expr))) (print ")")))]
                     [&&? (fn [expr] (do (print "(") (print-dnf (first (args expr))) (doall (map (fn [subexpr] (do (print " && ") (print-dnf subexpr))) (rest (args expr)))) (print ")")))]
                     [||? (fn [expr] (do (print "(") (print-dnf (first (args expr))) (doall (map (fn [subexpr] (do (print " || ") (print-dnf subexpr))) (rest (args expr)))) (print ")")))]
                     [no? (fn [expr] (do (print "!") (print-dnf (first (args expr)))))])]
    ((some (fn [rule] (if ((first rule) expr)
                        (second rule)
                        false))
           print-rules)
     expr)))

(defn println-dnf [expr]
  (do (print-dnf expr) (println)))

(defn get-first-disj-from-args [expr]
  (if (empty? expr)
    nil
    (let [first-arg (first expr)]
      (if (||? first-arg)
        first-arg
        (get-first-disj-from-args (rest expr))))))

(defn get-args-without-first-disj [expr]
  (if (empty? expr)
    '()
    (let [first-arg (first expr)]
      (if (||? first-arg)
        (rest expr)
        (cons first-arg (get-args-without-first-disj (rest expr)))))))

;; Для аргументов args выражения, удовлетворяющих pred, раскрывает скобки
;; Возвращает примененную операцию oper к модифицированным аргументам
(defn simplify-associativity [args pred oper]
  (apply oper (reverse (reduce
                        (fn [acc arg]
                          (if (pred arg)
                            (concat acc (rest arg))
                            (conj acc arg)))
                        '()
                        args))))

;; Раскрыть скобки для одинаковых операторов согласно закону ассоциативности
(defn simplify-brackets [expr]
  (let [simplify-rules (list
                        ;; Предикат - проверка на дизъюнкцию. Для всех аргументов-дизъюнкций раскрыть скобки
                        [||? (fn [expr] (apply || (args (simplify-associativity (map simplify-brackets (args expr)) ||? ||))))]
                        ;; Предикат - проверка на конъюнкцию. Для всех аргументов-конъюнкций раскрыть скобки
                        [&&? (fn [expr] (apply && (args (simplify-associativity (map simplify-brackets (args expr)) &&? &&))))]
                        ;; Предикат - проверка, что не дошли до листа
                        ;; Без преобразования - для упрощения выражений, которые могут находиться глубже
                        [(fn [expr] (not (or (constant? expr) (variable? expr)))) #(cons (first expr) (map simplify-brackets (args %)))]
                        [(fn [expr] true) (fn [expr] expr)])]
    ((some (fn [rule] (if ((first rule) expr)
                        (second rule)
                        false))
           simplify-rules)
     expr)))

(defn distribute [expr] ;; Применить зикон дистрибутивности
  (let [simplify-rules (list
                        ;; Предикат - проверка на конъюнкцию
                        ;; Преобразование - для первого аргумента-дизъюнкции раскрыть скобки
                        [&&? (fn [expr] (let [first-disj (get-first-disj-from-args (args expr))
                                              other-expr (apply && (get-args-without-first-disj (args expr)))]
                                          (if (= nil first-disj)
                                            (cons (first expr), (map distribute, (args expr)))
                                            (distribute (apply || (map (fn [expr] (distribute (&& expr other-expr))) (args first-disj)))))))]
                        ;; Предикат - проверка, что не дошли до листа
                        ;; Без преобразования - для упрощения выражений, которые могут находиться глубже
                        [(fn [expr] (not (or (constant? expr) (variable? expr)))) #(cons (first expr) (map distribute (args %)))]
                        [(fn [expr] true) (fn [expr] expr)])]
    ((some (fn [rule] (if ((first rule) expr)
                        (second rule)
                        false))
           simplify-rules)
     expr)))

;; Заменить все знаки отрицания, относящиейся ко всему выражению, знаками отрицания, относящимися к отдельным выражениям
(defn simplify-negatives [expr]
  (let [simplify-rules (list
                        ;; Предикат - проверка на отрицание; преобразование - && -> ||; || -> &&
                        ;; Выражения заменяются своими отрицаниями
                        [no? (fn [expr] (if (or (&&? (arg expr)) (||? (arg expr)))
                                          (let [negative-arguments (map (fn [expr] (simplify-negatives (no expr))), (args (arg expr)))]
                                            (if (&&? (arg expr))
                                              (apply || negative-arguments)
                                              (apply && negative-arguments)))
                                          (no (simplify-negatives (arg expr)))))]
                        ;; Предикат - проверка, что не дошли до листа
                        ;; Без преобразования - для упрощения выражений, которые могут находиться глубже
                        [(fn [expr] (not (or (constant? expr) (variable? expr)))) #(cons (first expr), (map simplify-negatives (args %)))]
                        [(fn [expr] true) (fn [expr] expr)])]
    ((some (fn [rule] (if ((first rule) expr)
                        (second rule)
                        false))
           simplify-rules)
     expr)))

;; Избавиться от всех логических операций, содержащихся в формуле, заменив их основными: конъюнкцией, дизъюнкцией, отрицанием
(defn simplify-extra-operations
  [expr]
  (let [simplify-rules (list
                        ;; Предикат - проверка на импликацию
                        ;; Преобразование - A -> B = !A || B
                        [->? (fn [expr] (|| (no (first (args expr))) (second (args expr))))]
                        ;; Предикат - проверка, что не дошли до листа
                        ;; Без преобразования - для упрощения выражений, которые могут находиться глубже
                        [(fn [expr] (not (or (constant? expr) (variable? expr)))) #(cons (first expr), (map simplify-extra-operations (args %)))]
                        [(fn [expr] true) (fn [expr] expr)])]
    ((some (fn [rule] (if ((first rule) expr)
                        (second rule)
                        false))
           simplify-rules)
     expr)))

(defn to-dnf [expr] ;; Приведение выражения к ДНФ
  (simplify-brackets (distribute (simplify-negatives (simplify-extra-operations expr)))))

(let [dnf1 (|| (variable :A), (variable :B))
      dnf2 (|| (&& (variable :A), (variable :B)), (no (variable :A)))
      dnf3 (|| (&& (variable :A), (variable :B), (no (variable :C)))
               (&& (no (variable :D)), (variable :E), (variable :F))
               (&& (variable :C), (variable :D))
               (variable :B))
      expr (no (|| (--> (variable :X), (variable :Y)), (no (--> (variable :Y), (variable :Z)))))]

  (println "DNF's:")
  (println-dnf dnf1)
  (println "->")
  (println-dnf (to-dnf dnf1))
  (println "-------------")
  (println-dnf dnf2)
  (println "->")
  (println-dnf (to-dnf dnf2))
  (println "-------------")
  (println-dnf dnf3)
  (println "->")
  (println-dnf (to-dnf dnf3))
  (println "-------------")
  (println "\nNot DNF's:")
  (println-dnf expr)
  (println "->")
  (println-dnf (simplify-extra-operations expr))
  (println "->")
  (println-dnf (simplify-negatives (simplify-extra-operations expr)))
  (println "->")
  (println-dnf (distribute (simplify-negatives (simplify-extra-operations expr))))
  (println "->")
  (println-dnf (to-dnf expr))
  (println "-------------")
  (println-dnf (to-dnf (&& (&& (variable :A) (variable :B)) (--> (variable :C) (variable :D)) (|| (variable :E) (variable :F)))))
  (println "->")
  (println-dnf (simplify-brackets (to-dnf (&& (&& (variable :A) (variable :B)) (--> (variable :C) (variable :D)) (|| (variable :E) (variable :F)))))))