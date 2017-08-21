﻿/*********************************************        
作者：曹旭升              
QQ：279060597
访问博客了解详细介绍及更多内容：   
http://blog.shengxunwei.com
**********************************************/
/*
 * Copyright ?2005, Patrik Bohman
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 *    - Redistributions of source code must retain the above copyright notice, 
 *      this list of conditions and the following disclaimer.
 *    - Redistributions in binary form must reproduce the above copyright notice, 
 *      this list of conditions and the following disclaimer in the documentation 
 *      and/or other materials provided with the distribution.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
 * OF SUCH DAMAGE.
 */
using System;
using System.Windows.Forms;
using System.Windows.Forms.Design; 
using System.Drawing.Design; 
using System.Drawing;
namespace Sheng.SailingEase.Controls.MozBar
{
	public class ImageMapEditor : System.Drawing.Design.UITypeEditor   
	{
		private IWindowsFormsEditorService wfes = null ;
		private int m_selectedIndex = -1 ;
		private ImageListPanel m_imagePanel = null ;
		public ImageMapEditor()
		{
		}
		protected virtual ImageList GetImageList(object component) 
		{
			if (component is MozItem.ImageCollection) 
			{
				return ((MozItem.ImageCollection) component).GetImageList();
			}
			return null ;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			wfes = (IWindowsFormsEditorService)	provider.GetService(typeof(IWindowsFormsEditorService));
			if((wfes == null) || (context == null))
				return null ;
			ImageList imageList = GetImageList(context.Instance) ;
			if ((imageList == null) || (imageList.Images.Count==0))
				return -1 ;
			m_imagePanel = new ImageListPanel(); 
			m_imagePanel.BackgroundColor = Color.FromArgb(241,241,241);
			m_imagePanel.BackgroundOverColor = Color.FromArgb(102,154,204);
			m_imagePanel.HLinesColor = Color.FromArgb(182,189,210);
			m_imagePanel.VLinesColor = Color.FromArgb(182,189,210);
			m_imagePanel.BorderColor = Color.FromArgb(0,0,0);
			m_imagePanel.EnableDragDrop = true;
			m_imagePanel.Init(imageList,12,12,6,(int)value);
			m_imagePanel.ItemClick += new ImageListPanelEventHandler(OnItemClicked);
			m_selectedIndex = -1;
			wfes.DropDownControl(m_imagePanel) ;
			return (m_selectedIndex != -1) ? m_selectedIndex : (int) value ;
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			if(context != null && context.Instance != null ) 
			{
				return UITypeEditorEditStyle.DropDown ;
			}
			return base.GetEditStyle (context);
		}
		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}
		public override void PaintValue(System.Drawing.Design.PaintValueEventArgs pe)
		{
			int imageIndex = -1 ;	
			if(pe.Value != null) 
			{
				try 
				{
					imageIndex = (int)Convert.ToUInt16( pe.Value.ToString() ) ;
				}
				catch
				{
				}
			}
			if((pe.Context.Instance == null) || (imageIndex < 0))
				return ;
			ImageList imageList = GetImageList(pe.Context.Instance) ;
			if((imageList == null) || (imageList.Images.Count == 0) || (imageIndex >= imageList.Images.Count))
				return ;
			pe.Graphics.DrawImage(imageList.Images[imageIndex],pe.Bounds);
		}
		public void OnItemClicked(object sender, ImageListPanelEventArgs e)
		{
			m_selectedIndex = ((ImageListPanelEventArgs) e).SelectedItem;
			m_imagePanel.ItemClick -= new ImageListPanelEventHandler(OnItemClicked);
			wfes.CloseDropDown() ;
			m_imagePanel.Dispose() ;
			m_imagePanel = null ;
		}
	}
}