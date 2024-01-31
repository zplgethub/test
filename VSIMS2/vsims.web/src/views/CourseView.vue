<template>
	<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="课程管理" sub-title="课程信息基本操作" />
	<a-form :model="formState" name="horizontal_query" layout="inline" autocomplete="off" @finish="onFinish" @finishFailed="onFinishFailed">
		<a-form-item label="课程名称" name="no">
			<a-input v-model:value="formState.courseName"></a-input>
		</a-form-item>
		<a-form-item label="老师" name="name">
			<a-input v-model:value="formState.teacher"></a-input>
		</a-form-item>
		<a-form-item>
			<a-button type="primary" html-type="submit">查询</a-button>
		</a-form-item>
		<a-form-item>
			<a-button type="primary" @click="addCourse">新增</a-button>
		</a-form-item>
	</a-form>
	<a-table :columns="columns" :data-source="dataSource" bordered :pagination="false" :row-key="record => record.id">
		<template #bodyCell="{ column, text, record }">
			<template v-if="[ 'name','teacher', 'createUser','lastEditUser'].includes(column.dataIndex)">
				<div>{{ text }}</div>
			</template>
			<template v-else-if="[ 'createTime','lastEditTime'].includes(column.dataIndex)">
				<div>{{formatDateString(text)}}</div>
			</template>
			<template v-else-if="column.dataIndex === 'operation'">
				<div class="editable-row-operations">
					<a @click="edit(record.id)">Edit</a>
				</div>
			</template>
		</template>
	</a-table>
	<a-pagination v-model:current="pagination.current" :total="pagination.total" @change="change" />
	<a-modal ref="modalRef" v-model:visible="visible" okText="保存" cancelText="取消" :wrap-style="{ overflow: 'hidden' }" @ok="handleOk">
		<div>
			<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="课程管理" sub-title="新增或编辑课程" />
			<br />
			<a-form :model="addEditFormState">
				<a-form-item label="课程名称">
					<a-input v-model:value="addEditFormState.name" />
				</a-form-item>
				<a-form-item label="授课老师">
					<a-input v-model:value="addEditFormState.teacher" />
				</a-form-item>
			</a-form>
		</div>
	</a-modal>
</template>
<script lang="ts">
	import {
		defineComponent,
		reactive,
		toRefs,
		ref,
		toRaw
	} from 'vue';
	import type {
		UnwrapRef
	} from 'vue';
	import { message } from 'ant-design-vue';
	import {
		getD,
		postD
	} from '../api/index.js';

	const columns = [
		{
			title: '课程名称',
			dataIndex: 'name',
			key: 'name',
			align: 'center',
			width: '10%',
		},
		{
			title: '老师',
			dataIndex: 'teacher',
			key: 'teacher',
			align: 'center',
			width: '10%',
		},
		{
			title: '创建人员',
			dataIndex: 'createUser',
			key: 'createUser',
			align: 'center',
			width: '10%',
		},
		{
			title: '创建时间',
			dataIndex: 'createTime',
			key: 'createTime',
			align: 'center',
			width: '10%',
		},
		{
			title: '编辑人员',
			dataIndex: 'lastEditUser',
			key: 'lastEditUser',
			align: 'center',
			width: '10%',
		},
		{
			title: '编辑时间',
			dataIndex: 'lastEditTime',
			key: 'lastEditTime',
			align: 'center',
			width: '10%',
		},
		{
			title: '操作',
			dataIndex: 'operation',
			key: 'operation',
			align: 'center',
			width: '10%',
		},
	];
	interface DataItem {
		id: number,
		name: string,
		teacher:string,
		createTime: Date,
		createUser: string,
		lastEditTime: Date,
		lastEditUser: string
	}
	interface FormState {
		courseName: string;
		teacher:string;
	}
	const pagination = {
		total: 1,
		current: 1,
		pageSize: 10,
	};
	const dataSource: DataItem[] = [];
	export default defineComponent({
		setup() {
			const formatDateString=(value)=>{
				var dt = new Date(value);
				let year = dt.getFullYear();
				let month = (dt.getMonth() + 1).toString().padStart(2,'0');
				let date = dt.getDate().toString().padStart(2,'0');
				let hour = dt.getHours().toString().padStart(2,'0');
				let minute = dt.getMinutes().toString().padStart(2,'0');
				let second = dt.getSeconds().toString().padStart(2,'0');
				return `${year}-${month}-${date} ${hour}:${minute}:${second}`;
			};
			const formState = reactive < FormState > ({
				courseName: '',
				teacher: '',
			});
			const addEditFormState = reactive({
				id: 0,
				name: '',
				teacher:''
			});
			const visible = ref < boolean > (false);
			const addCourse = () => {
				addEditFormState.id = -1;
				addEditFormState.name = '';
				addEditFormState.teacher='';
				visible.value = true;
			};

			const handleOk = (e: MouseEvent) => {
				console.log(e);
				console.log(addEditFormState);
				var url = "";
				if (addEditFormState.id >0) {
					url = "/api/Course/UpdateCourse"; //编辑
				} else {
					url = "/api/Course/AddCourse"; //新增
				}
				postD(url, {
					"id": addEditFormState.id>0?addEditFormState.id:null,
					"name": addEditFormState.name,
					"teacher": addEditFormState.teacher,
					"createTime": new Date(),
					"createUser": 0,
					"lastEditTime": new Date(),
					"lastEditUser": 0
				}).then(res => {
					console.log(res);
					if(res.status==200){
						if(res.data==0){
							message.success('保存成功！');
							visible.value = false;
							var courseName = formState.courseName;
							var teacher = formState.teacher;
							getCourses(courseName, teacher);
						}else{
							message.error('保存失败！');
						}
					}
				});
				
			};
			const onFinish = (values: any) => {
				var courseName = formState.courseName;
				var teacher = formState.teacher;
				getCourses(courseName, teacher);
				console.log('Success:', values);
			};

			const onFinishFailed = (errorInfo: any) => {
				console.log('Failed:', errorInfo);
			};
			const state = reactive({
				dataSource: dataSource
			});
			
			const getCourses = (courseName, teacher) => {
				dataSource.length = 0;
				getD('/api/Course/GetCourses', {
					"pageSize": pagination.pageSize,
					"pageNum": pagination.current,
					"courseName": courseName,
					"teacher": teacher
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						pagination.total = res.data.count; //总记录数
						console.log(pagination.total);
						for (let i = 0; i < res.data.items.length; i++) {
							dataSource.push({
								id: res.data.items[i].id,
								name: res.data.items[i].name,
								teacher: res.data.items[i].teacher,
								"createTime":res.data.items[i].createTime,
								"createUser":res.data.items[i].createUser,
								"lastEditTime":res.data.items[i].lastEditTime,
								"lastEditUser":res.data.items[i].lastEditUser,
							});
						}
						state.dataSource = [...dataSource];
					}
				});
			};
			getCourses(null,null);
			const editableData: UnwrapRef < Record < string, DataItem >> = reactive({});

			const edit = (id: number) => {
				console.log(id);
				var course = dataSource.filter(item => id === item.id)[0];
				addEditFormState.id = course.id;
				addEditFormState.name = course.name;
				addEditFormState.teacher = course.teacher;
				visible.value = true;
				console.log(course);
			};
			
			const change = (pagination) => {
				var courseName = formState.courseName;
				var teacher = formState.teacher;
				getCourses(courseName, teacher);
				console.log(pagination);
			};
			const onSubmit = () => {
				console.log('submit!', toRaw(formState));
			};
			return {
				formatDateString,
				formState,
				addEditFormState,
				...toRefs(state),
				columns,
				editingKey: '',
				editableData,
				edit,
				pagination,
				change,
				onFinish,
				onFinishFailed,
				onSubmit,
				visible,
				addCourse,
				handleOk,
			};
		},
	});
</script>
<style scoped>
	.editable-row-operations a {
		margin-right: 8px;
	}

	.ant-form {
		height: 6vh;
		width: 100vh;
		background-color: transparent;
	}

	.ant-modal-content {
		height: 50vh;
	}

	.ant-modal-body .ant-input {
		width: 40vh;
	}

	.ant-modal-body {
		height: 40vh;
	}

	.ant-modal-body .ant-page-header {
		width: 46vh;
		padding: 0.5rem;
	}
</style>
