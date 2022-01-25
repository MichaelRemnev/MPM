(defn my-map [func, coll]
  (reduce (fn [acc, elem] (concat acc (list (func, elem))))
   (list)
   coll))

(defn my-filter [pred, coll]
  (reduce (fn [acc, elem] (if (pred elem)
       (concat acc (list elem)) ; then
       acc)) ; else
   (list)
   coll))

(my-map inc '(1, 2, 3))
(my-filter even? (range 1, 10))