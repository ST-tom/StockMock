; (function ($, window, document, undefined) {
	var defaults = {
		getDataUrl: "",							//获取数据的请求路径
		pageIndex: 1,							//当前页码
		pageSize: 10,							//每页数据条目
		searchBtnId: "",						//搜索按钮Id
		searchClass: "",						//搜索项的类名
		tempId = "",							//模板Id
		isSearchParamChangedRefresh: false,     //是否当搜索项发生变化时就更新数据
		extendParams: null,				        //额外http请求参数
		afterSuccess: null,						//自定义http请求成功的处理
		beforeSend: null,						//自定义发送http请求前的处理
		error: null,							//自定义http请求失败处理
		complete: null,							//自定义http请求完成处理
		errorShow: null,						//自定义错误提示显示，错误信息参数errmsg - function(errmsg)
	}

	//页脚属性
	var footerConfig = {
		footerContainerId: "",      //页脚显示位置
		footerMaxPageShowSize: 5,   //页脚同时显示的页面条目
		footerFirstBtnShow: true,   //是否启用首末跳转页功能
		footerTurnBtnShow: true,   //是否启用跳转到指定页码功能
		totalCount: null,
		pageCount: null,
		footerHtml: [
			'<span class="page-footer">',
			'<span class="first-page">首页</span>',
			'<span class="pre-page">上一页</span>',
			'<span class="next-page">下一页</span>',
			'<span class="last-page">尾页</span>',
			'<span class="turnTo">',
			'第<input type="text" />页',
			'</span>',
			'<span class="confirm">确认</span>',
			'</span>',
		].join(''),
	}

	//私有方法
	var method = {
		defaultSuccess: function (data, that) {
			if (!data.result) {
				this.defaultErrorShow(that, "获取数据失败");
			} else {
				that.element.empty();
				that.element.append(data.htmlStr);
			}
		},
		defaulBuildContainer: function (data, that) {
			if (!data.result) {
				this.defaultErrorShow(that, "获取数据失败");
			}
			else {
				that.element.empty();


			}
		}
		defaultErrorShow: function (that, errmsg) {
			if ($.isFunction(that.options.errorShow)) {
				that.options.errorShow(errmsg);
			} else if (errmsg) {
				alert(errmsg);
			} else {
				alert("操作失败");
			}
		},
		initFooter: function (that) {
			if (!that.options.footerContainerId) {
				return false;
			}
			var $footer = $("#" + that.options.footerContainerId + "");
			$footer.append(that.options.footerHtml);

			this.buildFooterBtn(that);
			this.bindFooterEvent(that);
		},
		buildFooterBtn: function (that) {
			var $footer = $("#" + that.options.footerContainerId + "");
			$footer.find("a").remove();
			$footer.find(".totalPage").remove();

			that.options.pageCount = Math.ceil(that.options.totalCount / that.options.pageSize);
			var pageShowCount = that.options.pageCount <= that.options.footerMaxPageShowSize ? that.options.pageCount : that.options.footerMaxPageShowSize;

			docFragment = document.createDocumentFragment();
			for (var i = 1; i <= pageShowCount; i++) {
				var aNode = document.createElement('a');

				aNode.innerHTML = i + '';
				aNode.setAttribute("href", "javascription:void(0)")
				if (i == 1) {
					aNode.setAttribute('class', 'active');
				}

				docFragment.appendChild(aNode);
			}
			var html = '<span class="totalPage">共 ' + that.options.pageCount + ' 页</span>';
			$footer.find('.pre-page').after(docFragment);
			$footer.find('.last-page').after(html);

			method.switchPageIndex(that);
		},
		bindFooterEvent: function (that) {
			var $footer = $("#" + that.options.footerContainerId + "");
			var methods = this;
			//点击页面
			$footer.on('click', 'a', function () {
				$(this).addClass('active').siblings('a').removeClass('active');
				var index = window.parseInt($(this).text());
				that.options.pageIndex = index;
				methods.switchPageIndex(that);
			});
			//点击前一页，后一页
			$footer.on('click', '.pre-page', function () {
				var index = that.options.pageIndex;
				index--;
				that.options.pageIndex = index < 1 ? 1 : index;
				methods.switchPageIndex(that);
			});
			$footer.on('click', '.next-page', function () {
				var index = that.options.pageIndex;
				index++;
				that.options.pageIndex = index > that.options.pageCount ? that.options.pageCount : index;
				methods.switchPageIndex(that);
			});

			//首页和末页按钮
			if (that.options.footerFirstBtnShow) {
				$footer.on('click', '.first-page', function () {
					that.options.pageIndex = 1;
					methods.switchPageIndex(that);
				});
				$footer.on('click', '.last-page', function () {
					that.options.pageIndex = that.options.pageCount;
					methods.switchPageIndex(that);
				});
			} else {
				$footer.find('.first-page, .last-page').hide();
			}

			//调整按钮
			if (that.options.footerTurnBtnShow) {
				$footer.on('click', '.confirm', function () {
					var turnto = $footer.find(".turnTo input").val();
					$footer.find(".turnTo input").val('');

					if (!(parseInt(turnto) == turnto)) {
						methods.defaultErrorShow(that, "请输入整数");
						return false;
					}
					if (isNaN(turnto)) {
						methods.defaultErrorShow(that, "请输入整数");
						return false;
					}
					if ((turnto > that.options.pageCount) || (turnto < 1)) {
						methods.defaultErrorShow(that, "输入的页码超出范围");
						return false;
					}

					that.options.pageIndex = turnto;
					methods.switchPageIndex(that);
				});
			} else {
				$footer.find(".confirm, .turn").hide();
			}
		},
		switchPageIndex: function (that) {
			that.getData(that)
			this.updateFooter(that);
		},
		updateFooter: function (that) {
			$footer = $("#" + that.options.footerContainerId + "");
			if (that.options.totalCount == 0) {
				$footer.hide();
			} else {
				$footer.show();
			}

			var pageCount = that.options.pageCount,
				index = that.options.pageIndex,
				startIndex = 0,
				endIndex = 0,
				rightCount = parseInt(that.options.footerMaxPageShowSize / 2),
				footerMaxPageShowSize = that.options.footerMaxPageShowSize,
				$pageItems = $("#" + that.options.footerContainerId + "").find("a");

			if (pageCount > footerMaxPageShowSize) {
				if (index + rightCount <= pageCount) {
					endIndex = index + rightCount;
				} else {
					endIndex = pageCount;
				}
				endIndex = endIndex < footerMaxPageShowSize ? footerMaxPageShowSize : endIndex;//左侧开始的负数
				startIndex = endIndex - footerMaxPageShowSize + 1;
				$pageItems.each(function (index2) {
					$(this).text(startIndex + index2);
					if (startIndex + index2 === index) {
						$(this).addClass('active').siblings('a').removeClass('active');
					}
				});
			} else {
				$pageItems.each(function (index2) {
					if (index2 + 1 == index) {
						$(this).addClass('active').siblings('a').removeClass('active');
					}
				});
			}
		},
		hasValue: function (data) {
			if (data != null && data != "" && data != undefined) {
				return true;
			} else {
				return false;
			}
		}
	}

	//构造函数
	function TableList(element, opts) {
		this.element = element;
		this.options = $.extend({}, defaults, footerConfig, opts);
		this.init();
	}

	//原型(公开方法)
	Table.prototype = {
		init: function () {
			var that = this;
			that.bindEvent();
			that.getData(this, true);
			method.initFooter(this);
		},
		//绑定事件（搜索）
		bindEvent: function () {
			var that = this;
			if (method.hasValue(that.options.searchBtnId)) {
				$("#" + that.options.searchBtnId + "").bind("click", function () {
					that.getData(that, true);
				});
			}
			if (that.options.isSearchParamChangedRefresh) {
				if (method.hasValue(that.options.searchClass)) {
					$("." + that.options.searchClass + "").bind("change", function () {
						that.getData(that, true);
					});
				}
			}
		},
		getData: function (that, needRefresh) {
			if (needRefresh) {
				that.options.pageIndex = 1;
			}

			$.ajax({
				url: that.options.getDataUrl,
				type: "get",
				async: false,
				data: that.getSearchData(),
				beforeSend: function () {
					that.beforeSend();
				},
				success: function (data) {
					that.success(data);
					if (needRefresh) {
						if (data.result) {
							that.options.totalCount = data.totalCount;
							method.buildFooterBtn(that);
						}
					}
				},
				error: function () {
					that.error();
				},
				complete: function () {
					that.complete();
				},
			});
		},
		getSearchData: function () {
			var searchData = {};

			if (method.hasValue(this.options.searchClass)) {
				$("." + this.options.searchClass).each(function () {
					var type = $(this).attr("type");
					if (type == "radio" || type == "checkbox") {
						if ($(this).prop("checked")) {
							var key = $(this).attr("name");
							var value = $(this).val();
							if (!data[key]) {
								data[key] = [];
							}
							data[key].push(value);
						}
					} else {
						var key = $(this).attr("name");
						var value = $(this).val();
						searchData[key] = value;
					}
				});
			}

			searchData.pageIndex = this.options.pageIndex;
			searchData.pageSize = this.options.pageSize;

			return this.extendParams(searchData);
		},
		//额外自定义搜索数据
		extendParams: function (searchData) {
			if ($.isFunction(this.options.extendParams)) {
				this.options.extendParams(searchData);
			} else if (method.hasValue(this.options.extendParams)) {
				$.extend(searchData, this.options.extendParams);
			}
			return searchData;
		},
		beforeSend: function () {
			if ($.isFunction(this.options.beforeSend)) {
				this.options.beforeSend();
			}
		},
		success: function (data) {
			if ($.isFunction(this.options.success)) {
				this.options.success(data);
			} else {
				method.defaultSuccess(data, this);
			}
		},
		error: function () {
			if ($.isFunction(this.options.error)) {
				this.options.error();
			} else {
				method.defaultErrorShow(this, "获取数据失败");
			}
			return false;
		},
		complete: function () {
			if ($.isFunction(this.options.complete)) {
				this.options.complete();
			}
		},
		refreshFooter: function (needRefresh) {
			this.options.totalCount = null;
			this.options.pageIndex = 1;

			this.getData(this, needRefresh);
		}

	}

	$.fn.TSList = function (options) {
		return new TSList($(this), options);
	};
})(jQuery, window, document);