(defn my-filter [pred, coll]
  (reduce (fn [acc, elem]
            (if (pred, elem)
              (concat acc, (list elem))
              acc))
          (list), coll))

(defn divide-collection [n, acc, rest]
  (if (> (count rest), 0)
    (recur n, (concat acc (list (take n rest))), (drop n rest))
    acc))

(defn my-partition [n, coll]
  (divide-collection n, (list), coll))

;; (->> 5 (+ 3) (/ 2) (- 1))   ->  (- 1, (/ 2, (+ 3, 5)))

(defn my-filter-future [pred, coll, thread-number] 
  (->>
   (my-partition (Math/ceil (/ (count coll), thread-number)), coll)
   (map (fn [elem] (future (my-filter pred, elem))))
   (doall)
   (map deref)
   (reduce concat)))


;;(doall (my-filter even? (range 0, 100)))
;;(doall (my-filter-future even? (range 0, 50), 4))

;;(time (doall (filter even? (range 0, 1000))))
(time (doall (my-filter even? (range 0, 1000))))
;;(time (doall (my-filter-future even? (range 0, 1000), 4)))