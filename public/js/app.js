/*global Vue, todoStorage */

(function (exports) {

	'use strict'

	const todoStorage = { // todo API的功能物件
		get: function (vm, cb) { // 呼叫API，取todo清單
			fetch('/api/todo').then((res) => res.json().then((data) => cb(vm, data)))
		},
		add: function (vm, title, cb) { // 呼叫API，新增todo
			fetch('/api/todo', {
				method: 'POST', // 使用POST方式呼叫API
				body: JSON.stringify(title), // 輸入的標題資料轉成JSON
				headers: { 'Content-Type': 'application/json' } // 設定資料為json格式
			}).then(res => res.json().then((data) => cb(vm, data)))
		},
		save: function (vm, todo, cb) { // 呼叫API，儲存指定todo
			fetch(`/api/todo/${todo.id}`, {
				method: 'PUT', // 使用PUT方式呼叫API
				body: JSON.stringify(todo), // 把todo轉成json格式
				headers: { 'Content-Type': 'application/json' } // 設定資料為json格式
			}).then((res) => res.json().then((data) => cb(vm, data)))
		},
		remove: function (vm, todo, cb) { // 呼叫API，刪除指定todo
			fetch(`/api/todo/${todo.id}`, {
				method: 'DELETE', // 使用DELTE方式呼叫API
			}).then((res) => res.json().then((data) => cb(vm, data)))
		},
		removeCompleted: function (vm, cb) { // 呼叫API，刪除全部「已完成」的todo
			fetch(`/api/todo/completed`, {
				method: 'DELETE', // 使用DELTE方式呼叫API
			}).then((res) => res.json().then((data) => cb(vm, data)))
		},
	}

	const auth = { // 身分驗證 API的功能物件
		current: function (vm, cb) { // 呼叫API，取目前登入者
			fetch('/api/user/current').then(res => res.json().then((data) => cb(vm, data)))
		},
		register: function (username, cb) { // 呼叫API，註冊使用者
			fetch('/api/user/register', {
				method: 'POST', // 使用POST方式呼叫API
				body: JSON.stringify(username), // 輸入的帳號資料轉成JSON
				headers: { 'Content-Type': 'application/json' } // 設定資料為json格式
			}).then(res => res.json().then((data) => cb(data)))
		},
		login: function (username, cb) { // 呼叫API，登入使用者
			fetch('/api/user/login', {
				method: 'POST', // 使用POST方式呼叫API
				body: JSON.stringify(username), // 輸入的帳號資料轉成JSON
				headers: { 'Content-Type': 'application/json' } // 設定資料為json格式
			}).then(res => res.json().then((data) => cb(data)))
		},
		logout: function (cb) { // 呼叫API，登出使用者
			fetch('/api/user/logout', {
				method: 'DELETE', // 使用DELETE方式呼叫API
			}).then(res => {
				cb(res.ok)
			})
		},
	}

	var filters = {
		all: function (todos) { // 取全部的todo
			return todos
		},
		active: function (todos) { // 過濾未完成的todo
			return todos.filter(function (todo) {
				return !todo.completed
			})
		},
		completed: function (todos) { // 過濾已完成的todo
			return todos.filter(function (todo) {
				return todo.completed
			})
		}
	}

	exports.app = new Vue({

		// the root element that will be compiled
		el: '.todoapp',

		// app initial state
		data: {
			todos: [], // 儲存todo資料
			newTodo: '', // 新增todo用的變數
			editedTodo: null, // 編輯todo用的變數
			visibility: 'all', // 目前todo清單的過濾條件
			user: {}, // 紀錄目前登入者
			username: '',// 登入或註冊用的變數
			loading: false, // 紀錄呼叫API的狀態
		},

		// computed properties
		// http://vuejs.org/guide/computed.html
		computed: {
			filteredTodos: function () { // 取目前過濾後的todo清單
				return filters[this.visibility](this.todos)
			},
			remaining: function () { // 取目前未完成的todo數量
				return filters.active(this.todos).length
			}
		},

		// methods that implement data logic.
		// note there's no DOM manipulation here at all.
		methods: {
			login: function () { // 登入功能
				if (this.user.username) {
					window.alert(`${this.user.username}, 你已經登入囉`)
					return
				}

				if (this.username === '') {
					return
				}

				if (this.loading) {
					window.alert('登入中')
					return
				}
				this.loading = true
				auth.login(this.username, (user) => {
					if (user.username) {
						this.user = user
						this.username = ''
						this.refreshTodo()
					} else {
						window.alert('登入失敗，請確認帳號有沒有打錯，或者先註冊帳號')
					}
					this.loading = false
				})
			},

			signup: function () { // 註冊功能
				if (this.user.username) {
					return
				}

				if (this.username === '') {
					return
				}

				if (this.loading) {
					window.alert('註冊中')
					return
				}

				auth.register(this.username, (user) => {
					if (user.username) {
						this.user = user
						this.username = ''
						this.refreshTodo()
					} else {
						window.alert('註冊失敗')
					}
					this.loading = false
				})
			},

			logout: function () { // 登出功能
				if (this.user.username) {
					auth.logout(() => {
						this.user = {}
						this.refreshTodo()
					})
					return
				}
			},

			pluralize: function (word, count) { // todo清單顯示數量功能
				return word + (count === 1 ? '' : 's')
			},

			refreshTodo() { // 刷新todo清單功能
				// 呼叫API，增加todo，存到網頁中
				todoStorage.get(this, function (vm, data) {
					vm.todos = data
				})
			},

			addTodo: function () { // 新增todo功能
				var value = this.newTodo && this.newTodo.trim()
				if (!value) {
					return
				}

				todoStorage.add(this, value, function (vm, data) {
					vm.todos = data
					vm.newTodo = ''
				})
			},

			removeTodo: function (todo) { // 移除todo功能
				todoStorage.remove(this, todo, function (vm, data) {
					vm.todos = data
				})
			},

			editTodo: function (todo) { // 使用者開始編輯todo的動作
				this.beforeEditCache = todo.title
				this.editedTodo = todo
			},

			doneEdit: function (todo) { // 使用者完成todo的編輯動作
				if (!this.editedTodo) {
					return
				}
				this.editedTodo = null
				todo.title = todo.title.trim()
				if (!todo.title) { // 如果沒有輸入標題了，則刪除
					this.removeTodo(todo)
				} else { // 如果有輸入標題，則更新
					todoStorage.save(this, todo, function (vm, data) {
						vm.todos = data
					})
				}
			},

			cancelEdit: function (todo) { // 使用者取消編輯todo的功能
				this.editedTodo = null
				todo.title = this.beforeEditCache
			},

			completeTodo: function (todo) { // 使用者點擊todo的完成動作功能
				todo.completed = !todo.completed
				todoStorage.save(this, todo, function (vm, data) {
					vm.todos = data
				})
			},

			removeCompleted: function () { // 刪除全部已完成todo的功能
				todoStorage.removeCompleted(this, function (vm, data) {
					vm.todos = data
				})
			}
		},

		mounted: function () { // 初次載入功能
			console.log("Mounted")

			auth.current(this, (vm, user) => vm.user = user) // 取目前登入者資料
			this.refreshTodo() // 刷新todo
		},

		// a custom directive to wait for the DOM to be updated
		// before focusing on the input field.
		// http://vuejs.org/guide/custom-directive.html
		directives: {
			'todo-focus': function (el, binding) {
				if (binding.value) {
					el.focus()
				}
			}
		}
	})

})(window)
