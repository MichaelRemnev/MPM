(ns task4-test
  (:use task4)
  (:require [clojure.test :as test]))

(test/deftest task4-test
  ;; Константы
  (test/testing
    (test/is (constant? (constant 1)))
    (test/is (not (constant? (list 1, 2))))
    (test/is (= 1, (constant-value (constant 1)))))
              
  ;; Переменные
  (test/testing
    (test/is (variable? (variable :x)))
    (test/is (not (variable? (list 1, 2))))
    (test/is (= :x, (variable-name (variable :x))))
    (test/is (same-variables? (variable :x), (variable :x)))
    (test/is (not (same-variables? (variable :x), (variable :y)))))
              
  ;; Операции
  (test/testing
    (test/is (&&? (&& 1, 2)))
    (test/is (not (&&? (list 1, 2))))

    (test/is (||? (|| 1, 2)))
    (test/is (not (||? (list 1, 2))))

    (test/is (no? (no (list 1, 2))))
    (test/is (not (no? (list 1, 2))))
    (test/is (not (no? (no (no (list 1, 2))))))

    (test/is (= '(1), (args (constant 1))))
    (test/is (= '(1 2), (args (&& 1, 2))))
    (test/is (= '(1 2), (args (|| 1, 2))))
    (test/is (= '((1 2)), (args (no '(1, 2)))))
    (test/is (= '(1 2), (arg (no '(1, 2))))))
  
  ;; get-first-disj-from-args
  (test/testing 
    (test/is (= nil, (get-first-disj-from-args (list (&& (variable :A), (variable :B))))))
    (test/is (= nil, (get-first-disj-from-args (list (&& (variable :A), (variable :B)), (&& (variable :C), (variable :D))))))
    (test/is (= (|| (variable :C), (variable :D)), (get-first-disj-from-args (list (&& (variable :A), (variable :B)), (|| (variable :C), (variable :D))))))
    (test/is (= (|| (variable :A), (variable :B)), (get-first-disj-from-args (list (|| (variable :A), (variable :B)), (|| (variable :C), (variable :D)))))))
  
  ;; get-args-without-first-disj
  (test/testing 
    (test/is (= '(), (get-args-without-first-disj (list (|| (variable :A), (variable :B))))))
    (test/is (= (list (&& (variable :A), (variable :B))), (get-args-without-first-disj (list (&& (variable :A), (variable :B))))))
    (test/is (= (list (&& (variable :A), (variable :B)), (&& (variable :C), (variable :D))), (get-args-without-first-disj (list (&& (variable :A), (variable :B)), (&& (variable :C), (variable :D))))))
    (test/is (= (list (&& (variable :A), (variable :B))), (get-args-without-first-disj (list (&& (variable :A), (variable :B)), (|| (variable :C), (variable :D))))))
    (test/is (= (list (|| (variable :C), (variable :D))), (get-args-without-first-disj (list (|| (variable :A), (variable :B)), (|| (variable :C), (variable :D)))))))
  (let [dnf1 (|| (variable :A), (variable :B))
        dnf2 (|| (&& (variable :A), (variable :B)), (no (variable :A)))
        dnf3 (|| (&& (variable :A), (variable :B), (no (variable :C)))
                 (&& (no (variable :D)), (variable :E), (variable :F))
                 (&& (variable :C), (variable :D))
                 (variable :B))
        expr (no (|| (--> (variable :X), (variable :Y)), (no (--> (variable :Y), (variable :Z)))))
        expr-oper (no (|| (|| (no (variable :X)), (variable :Y)), (no (|| (no (variable :Y)), (variable :Z)))))
        expr-!neg (&& (&& (variable :X), (no (variable :Y))), (|| (no (variable :Y)), (variable :Z)))
        expr-dstr (|| (&& (no (variable :Y)), (&& (variable :X), (no (variable :Y)))), (&& (variable :Z), (&& (variable :X), (no (variable :Y)))))
        expr-dnf (|| (&& (no (variable :Y)), (variable :X), (no (variable :Y))), (&& (no (variable :Y)), (variable :X), (variable :Z)))

        expr2 (&& (&& (variable :A), (variable :B)), (--> (variable :C), (variable :D)), (|| (variable :E), (variable :F)))
        expr2-oper (&& (&& (variable :A), (variable :B)), (|| (no (variable :C)), (variable :D)), (|| (variable :E), (variable :F)))
        expr2-!neg (&& (&& (variable :A), (variable :B)), (|| (no (variable :C)), (variable :D)), (|| (variable :E), (variable :F)))
        expr2-dstr (|| (|| (&& (&& (variable :E), (&& (variable :A), (variable :B))), (no (variable :C))), (&& (&& (variable :F), (&& (variable :A), (variable :B))), (no (variable :C))))
                       (|| (&& (&& (variable :E), (&& (variable :A), (variable :B))), (variable :D)), (&& (&& (variable :F), (&& (variable :A), (variable :B))), (variable :D))))
        expr2-dnf (|| (&& (variable :F), (variable :A), (variable :B), (variable :D))
                      (&& (variable :E), (variable :A), (variable :B), (variable :D))
                      (&& (variable :F), (variable :A), (variable :B), (no (variable :C)))
                      (&& (variable :E), (variable :A), (variable :B), (no (variable :C))))]
    
    ;; simplify-extra-operations
    (test/testing 
      (test/is (= dnf1 (simplify-extra-operations dnf1)))
      (test/is (= dnf2 (simplify-extra-operations dnf2)))
      (test/is (= dnf3 (simplify-extra-operations dnf3)))
      (test/is (= expr-oper (simplify-extra-operations expr)))
      (test/is (= expr2-oper (simplify-extra-operations expr2))))
    
    ;; simplify-negatives
    (test/testing 
      (test/is (= dnf1 (simplify-negatives (simplify-extra-operations dnf1))))
      (test/is (= dnf2 (simplify-negatives (simplify-extra-operations dnf2))))
      (test/is (= dnf3 (simplify-negatives (simplify-extra-operations dnf3))))
      (test/is (= expr-!neg (simplify-negatives (simplify-extra-operations expr))))
      (test/is (= expr2-!neg (simplify-negatives (simplify-extra-operations expr2)))))
    
    ;; distribute
    (test/testing
      (test/is (= dnf1 (distribute (simplify-negatives (simplify-extra-operations dnf1)))))
      (test/is (= dnf2 (distribute (simplify-negatives (simplify-extra-operations dnf2)))))
      (test/is (= dnf3 (distribute (simplify-negatives (simplify-extra-operations dnf3)))))
      (test/is (= expr-dstr (distribute (simplify-negatives (simplify-extra-operations expr)))))
      (test/is (= expr2-dstr (distribute (simplify-negatives (simplify-extra-operations expr2))))))
    
    ;; to-dnf
    (test/testing
      (test/is (= dnf1 (to-dnf dnf1)))
      (test/is (= dnf2 (to-dnf dnf2)))
      (test/is (= dnf3 (to-dnf dnf3)))
      (test/is (= expr-dnf (to-dnf expr)))
      (test/is (= expr2-dnf (to-dnf expr2))))))

(test/run-tests 'task4-test)